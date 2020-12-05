using Configuration;
using DbUp;
using System;

namespace DbStructure
{
	public interface IInitializeDatabase
	{
		string CreateUpdateDb();
		string Result { get; }
	}

	public class InitializeDatabase : IInitializeDatabase
	{
		private readonly string _connectionString;
		private InitializeDatabase() { }
		public InitializeDatabase(IConfigurationManager configuration)
		{
			_connectionString = configuration.GetConnectionWriterString();
		}

		public string CreateUpdateDb()
		{
			try
			{
				EnsureDatabase.For.MySqlDatabase(_connectionString);

				var upgrader = DeployChanges.To.MySqlDatabase(_connectionString)
					.WithScriptsEmbeddedInAssembly(typeof(InitializeDatabase).Assembly,
					(string s) => s.StartsWith("DbStructure.Sql"))
					.LogToNowhere()
					.Build();

				var result = upgrader.PerformUpgrade();

				if (result.Successful)
				{
					Result = null;
				}
				else
				{
					Result = result.Error.Message;
				}

			}
			catch(Exception ex)
			{
				Result = ex.Message;
			}

			return Result;
		}

		public string Result {private set; get;}
	}
}
