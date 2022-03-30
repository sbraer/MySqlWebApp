using Configuration;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using MySqlIdentityModel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MySqlIdentityDal
{
	public class RoleStore : IRoleStore<ApplicationRole>
	{
		private readonly string _connectionReaderString = null!;
		private readonly string _connectionWriterString = null!;
		private RoleStore() { }
		
		public RoleStore(IConfigurationManager configurationManager)
		{
			_connectionReaderString = configurationManager.GetConnectionReaderString();
			_connectionWriterString = configurationManager.GetConnectionWriterString();
		}

		public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(role.Id))
			{
				role.Id = Guid.NewGuid().ToString();
			}

			using var conn = new MySqlConnection(_connectionWriterString);
			await conn.OpenAsync(cancellationToken);
			using var comm = new MySqlCommand(
				"insert into AspNetRoles(Id,Name,NormalizedName) values(@id,@name,@normalizedName);"
				, conn);
			comm.Parameters.AddRange(
				new MySqlParameter[]
				{
					new MySqlParameter("@id", MySqlDbType.VarChar,256)
					{
						Value = role.Id != null? role.Id : Guid.NewGuid().ToString()
					},
					new MySqlParameter("@name", MySqlDbType.VarChar,256)
					{
						Value = role.Name
					},
					new MySqlParameter("@normalizedName", MySqlDbType.VarChar,256)
					{
						Value = role.NormalizedName!=null?role.NormalizedName:role.Name.ToUpper()
					}
				});

			int values = await comm.ExecuteNonQueryAsync(cancellationToken);
			if (values != 1)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Error in insert new role in table" });
			}
			else
			{
				return IdentityResult.Success;
			}
		}

		public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			using var conn = new MySqlConnection(_connectionWriterString);
			await conn.OpenAsync(cancellationToken);
			using var comm = new MySqlCommand(
				"delete from AspNetRoles where Id = @id;"
				, conn);
			comm.Parameters.AddRange(
				new MySqlParameter[]
				{
					new MySqlParameter("@id", MySqlDbType.VarChar,256)
					{
						Value = role.Id
					}
				});

			int values = await comm.ExecuteNonQueryAsync(cancellationToken);
			if (values != 1)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Error in delete role in table" });
			}
			else
			{
				return IdentityResult.Success;
			}
		}

		public void Dispose()
		{
			//throw new NotImplementedException();
		}

		public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			using var conn = new MySqlConnection(_connectionReaderString);
			await conn.OpenAsync(cancellationToken);
			using var comm = new MySqlCommand("select Id,Name,NormalizedName from AspNetRoles where Id=@id;", conn);
			comm.Parameters.Add(new MySqlParameter("@id", MySqlDbType.VarChar,256)
			{
				Value = roleId
			});

			using var re = await comm.ExecuteReaderAsync(cancellationToken);
			if (await re.ReadAsync())
			{
				return new ApplicationRole
				{
					Id = re.GetString(0),
					Name = re.GetString(1),
					NormalizedName = re.GetString(2)
				};
			}

			return null;
		}

		public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			using var conn = new MySqlConnection(_connectionReaderString);
			await conn.OpenAsync(cancellationToken);
			using var comm = new MySqlCommand("select Id,Name,NormalizedName from AspNetRoles where NormalizedName=@name;", conn);
			comm.Parameters.Add(new MySqlParameter("@name", MySqlDbType.VarChar, 256)
			{
				Value = normalizedRoleName
			});

			using var re = await comm.ExecuteReaderAsync(cancellationToken);
			if (await re.ReadAsync(cancellationToken))
			{
				return new ApplicationRole
				{
					Id = re.GetString(0),
					Name = re.GetString(1),
					NormalizedName = re.GetString(2)
				};
			}

			return null;
		}

		public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.NormalizedName);
		}

		public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.Id);
		}

		public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.Name);
		}

		public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
		{
			role.NormalizedName = normalizedName;
			return Task.CompletedTask;
		}

		public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
		{
			role.Name = roleName;
			return Task.CompletedTask;
		}

		public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			using var conn = new MySqlConnection(_connectionWriterString);
			await conn.OpenAsync(cancellationToken);
			using var comm = new MySqlCommand(
				"update AspNetRoles set Name=@name,NormalizedName=@normalizedName) where Id = @id;"
				, conn);
			comm.Parameters.AddRange(
				new MySqlParameter[]
				{
					new MySqlParameter("@id", MySqlDbType.VarChar,256)
					{
						Value = role.Id != null? role.Id : Guid.NewGuid().ToString()
					},
					new MySqlParameter("@name", MySqlDbType.VarChar,256)
					{
						Value = role.Name
					},
					new MySqlParameter("@normalizedName", MySqlDbType.VarChar,256)
					{
						Value = role.NormalizedName!=null?role.NormalizedName:role.Name.ToUpper()
					}
				});

			int values = await comm.ExecuteNonQueryAsync(cancellationToken);
			if (values != 1)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Error in update role in table" });
			}
			else
			{
				return IdentityResult.Success;
			}
		}
	}
}
