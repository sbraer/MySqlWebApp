using Configuration;
using Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySqlIdentityDal
{
	public class ArticleStore : IArticleStore
	{
		private readonly string _connectionWriterString = null;
		private readonly string _connectionReaderString = null;

		public ArticleStore(IConfigurationManager configuration)
		{
			_connectionWriterString = configuration.GetConnectionWriterString();
			_connectionReaderString = configuration.GetConnectionReaderString();
		}

		public async Task<List<Article>> GetAllArticle()
		{
			using var conn = new MySqlConnection(_connectionReaderString);
			await conn.OpenAsync();
			using var comm = new MySqlCommand("select Id,Name,Title from Articles order by Id asc;", conn);
			using var re = await comm.ExecuteReaderAsync();
			var coll = new List<Article>();
			while (await re.ReadAsync())
			{
				coll.Add(new Article
				{
					Id = re.GetInt32(0),
					Name = re.GetString(1),
					Title = re.GetString(2)
				});
			}

			return coll;
		}

		public async Task<Article> GetArticle(int id)
		{
			using var conn = new MySqlConnection(_connectionReaderString);
			await conn.OpenAsync();
			using var comm = new MySqlCommand("select Id,Name,Title,ArticleText from Articles where Id=@id;", conn);
			comm.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)
			{
				Value = id
			});

			using var re = await comm.ExecuteReaderAsync();
			if (await re.ReadAsync())
			{
				return new Article
				{
					Id = re.GetInt32(0),
					Name = re.GetString(1),
					Title = re.GetString(2),
					ArticleText = re.GetString(3)
				};
			}

			return null;
		}

		public async Task UpdateArticle(int? id, string title, string name, string articletext)
		{
			string sqlCommand = id.HasValue ?
				"update Articles set title=@title, name=@name, Articletext=@articletext where id=@id;"
				:
				"insert into Articles(Title,Name,ArticleText) values(@title,@name,@articletext);";

			using var conn = new MySqlConnection(_connectionWriterString);
			await conn.OpenAsync();
			using var comm = new MySqlCommand(sqlCommand, conn);
			comm.Parameters.AddRange(
				new MySqlParameter[]
				{
					new MySqlParameter("@id", MySqlDbType.Int32)
					{
						Value = id
					},
					new MySqlParameter("@title", MySqlDbType.VarChar, 50)
					{
						Value = title
					},
					new MySqlParameter("@name", MySqlDbType.VarChar,50)
					{
						Value = name
					},
					new MySqlParameter("@articletext", MySqlDbType.VarChar, 10000)
					{
						Value = articletext
					},
				});

			int values = await comm.ExecuteNonQueryAsync();
			if (values != 1)
			{
				throw new Exception("Error in insert/update data in table");
			}
		}
	}
}
