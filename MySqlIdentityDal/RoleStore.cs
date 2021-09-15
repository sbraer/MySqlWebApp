using Configuration;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using MySqlIdentityModel;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MySqlIdentityDal
{
	public class RoleStore : IRoleStore<ApplicationRole>, IRoleClaimStore<ApplicationRole>
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

		public async Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
		{
			await Task.Yield();
			role.NormalizedName = normalizedName;
		}

		public async Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
		{
			await Task.Yield();
			role.Name = roleName;
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

		public async Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default)
		{
			using var conn = new MySqlConnection(_connectionReaderString);
			await conn.OpenAsync(cancellationToken);

			using var comm = new MySqlCommand("SELECT uc.ClaimType, uc.ClaimValue FROM AspNetRoles u inner join AspNetRoleClaims uc on u.Name = uc.RoleId where u.NormalizedName = @rolename", conn);
			comm.Parameters.Add(new MySqlParameter("@rolename", MySqlDbType.VarChar, 256)
			{
				Value = role.Id
			});

			using var re = await comm.ExecuteReaderAsync(cancellationToken);

			var coll = new List<Claim>();
			while (await re.ReadAsync(cancellationToken))
			{
				coll.Add(new Claim(re.GetString(0), re.GetString(1)));
			}

			return coll;
		}

		public async Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
		{
			using var conn = new MySqlConnection(_connectionWriterString);
			await conn.OpenAsync(cancellationToken);

			using var comm = new MySqlCommand("insert into AspNetRoleClaims(RoleId, ClaimType, ClaimValue) values(@roleid, @claimtype, @claimvalue)", conn);
			comm.Parameters.AddRange(new MySqlParameter[] {
				new MySqlParameter("@roleid", MySqlDbType.VarChar, 256)
				{
					Value = role.Id
				},
				new MySqlParameter("@claimtype", MySqlDbType.LongText)
				{
					Value = claim.Type
				},
				new MySqlParameter("@claimvalue", MySqlDbType.LongText)
				{
					Value = claim.Value
				}
			});

			int result = await comm.ExecuteNonQueryAsync();
			if (result != 1)
			{
				throw new ArgumentException("Error in insert operation in claims role table");
			}
		}

		public async Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
		{
			using var conn = new MySqlConnection(_connectionWriterString);
			await conn.OpenAsync(cancellationToken);

			using var comm = new MySqlCommand("delete from AspNetRoleClaims where RoleId = @roleid and ClaimType = @claimtype and ClaimValue = @claimvalue)", conn);
			comm.Parameters.AddRange(new MySqlParameter[] {
				new MySqlParameter("@roleid", MySqlDbType.VarChar, 256)
				{
					Value = role.Id
				},
				new MySqlParameter("@claimtype", MySqlDbType.LongText)
				{
					Value = claim.Type
				},
				new MySqlParameter("@claimvalue", MySqlDbType.LongText)
				{
					Value = claim.Value
				}
			});

			int result = await comm.ExecuteNonQueryAsync();
			if (result != 1)
			{
				throw new ArgumentException("Error in delete claims");
			}
		}
	}
}
