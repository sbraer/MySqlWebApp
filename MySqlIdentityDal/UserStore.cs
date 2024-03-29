﻿using Configuration;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using MySqlIdentityModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MySqlIdentityDal
{
	public class UserStore : IUserStore<ApplicationUser>,
									IUserEmailStore<ApplicationUser>,
									IUserPasswordStore<ApplicationUser>,
									IUserLoginStore<ApplicationUser>,
									IUserRoleStore<ApplicationUser>
	{
        private readonly string _connectionReaderString = null!;
        private readonly string _connectionWriterString = null!;
        private UserStore() {}
        public UserStore(IConfigurationManager configurationManager)
        {
            _connectionReaderString = configurationManager.GetConnectionReaderString();
            _connectionWriterString = configurationManager.GetConnectionWriterString();
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = Guid.NewGuid().ToString();
            }

            using var conn = new MySqlConnection(_connectionWriterString);
            await conn.OpenAsync(cancellationToken);
            using var comm = new MySqlCommand(
                "insert into AspNetUsers(Id,UserName,NormalizedUserName,Email,NormalizedEmail,PasswordHash) values(@id,@username,@normalizedUsername,@email,@normalizedemail,@passwordHash)"
                , conn);
            comm.Parameters.AddRange(
                new MySqlParameter[]
                {
                    new MySqlParameter("@id", MySqlDbType.VarChar,256)
                    {
                        Value = user.Id
                    },
                    new MySqlParameter("@username", MySqlDbType.VarChar,256)
                    {
                        Value = user.UserName
                    },
                    new MySqlParameter("@normalizedUsername", MySqlDbType.VarChar,256)
                    {
                        Value = user.NormalizedUserName!=null?user.NormalizedUserName:user.UserName.ToUpper()
                    },
                    new MySqlParameter("@email", MySqlDbType.VarChar, 256)
                    {
                        Value = user.Email
                    },
                    new MySqlParameter("@normalizedemail", MySqlDbType.VarChar, 256)
                    {
                        Value = user.NormalizedEmail
                    },
                    new MySqlParameter("@passwordHash", MySqlDbType.LongText)
                    {
                        Value = user.PasswordHash
                    }
                });

            int values = await comm.ExecuteNonQueryAsync(cancellationToken);
            if (values != 1)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Error in insert new user in table" });
            }
            else
            {
                return IdentityResult.Success;
            }
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using var conn = new MySqlConnection(_connectionWriterString);
            await conn.OpenAsync(cancellationToken);
            using var comm = new MySqlCommand(
                "update AspNetUsers set UserName=@username,NormalizedUserName=@normalizedUsername,Email=@email,NormalizedEmail=@normalizedemail, PasswordHash=@passwordHash where Id = @id;"
                , conn);
            comm.Parameters.AddRange(
                new MySqlParameter[]
                {
                    new MySqlParameter("@id", MySqlDbType.VarChar,256)
                    {
                        Value = user.Id
                    },
                    new MySqlParameter("@username", MySqlDbType.VarChar,256)
                    {
                        Value = user.UserName
                    },
                    new MySqlParameter("@normalizedUsername", MySqlDbType.VarChar,256)
                    {
                        Value = user.NormalizedUserName!=null?user.NormalizedUserName:user.UserName.ToUpper()
                    },
                    new MySqlParameter("@email", MySqlDbType.VarChar, 256)
                    {
                        Value = user.Email
                    },
                    new MySqlParameter("@normalizedemail", MySqlDbType.VarChar, 256)
                    {
                        Value = user.NormalizedEmail
                    },
                    new MySqlParameter("@passwordHash", MySqlDbType.LongText)
                    {
                        Value = user.PasswordHash
                    }
                });

            int values = await comm.ExecuteNonQueryAsync(cancellationToken);
            if (values != 1)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Error in update user in table" });
            }
            else
            {
                return IdentityResult.Success;
            }
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
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
                        Value = user.Id
                    }
                });

            int values = await comm.ExecuteNonQueryAsync(cancellationToken);
            if (values != 1)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Error in delete user in table" });
            }
            else
            {
                return IdentityResult.Success;
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using var conn = new MySqlConnection(_connectionReaderString);
            await conn.OpenAsync(cancellationToken);
            using var comm = new MySqlCommand("select Id,UserName,NormalizedUserName,Email,NormalizedEmail,PasswordHash from AspNetUsers where Id=@id;", conn);
            comm.Parameters.Add(new MySqlParameter("@id", MySqlDbType.VarChar, 256)
            {
                Value = userId
            });

            using var re = await comm.ExecuteReaderAsync(cancellationToken);
            if (await re.ReadAsync())
            {
                return new ApplicationUser
                {
                    Id = re.GetString(0),
                    UserName = re.GetString(1),
                    NormalizedUserName = re.GetString(2),
                    Email = re.GetString(3),
                    NormalizedEmail = re.GetString(4),
                    PasswordHash = re.GetString(5)
                };
            }

            return null;
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using var conn = new MySqlConnection(_connectionReaderString);
            await conn.OpenAsync(cancellationToken);
            using var comm = new MySqlCommand("select Id,UserName,NormalizedUserName,Email,NormalizedEmail,PasswordHash from AspNetUsers where NormalizedUserName=@name;", conn);
            comm.Parameters.Add(new MySqlParameter("@name", MySqlDbType.VarChar, 256)
            {
                Value = normalizedUserName
            });

            using var re = await comm.ExecuteReaderAsync(cancellationToken);
            if (await re.ReadAsync())
            {
                return new ApplicationUser
                {
                    Id = re.GetString(0),
                    UserName = re.GetString(1),
                    NormalizedUserName = re.GetString(2),
                    Email = re.GetString(3),
                    NormalizedEmail = re.GetString(4),
                    PasswordHash = re.GetString(5)
                };
            }

            return null;
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            using var conn = new MySqlConnection(_connectionReaderString);
            await conn.OpenAsync(cancellationToken);
            using var comm = new MySqlCommand("select Id,UserName,NormalizedUserName,Email,NormalizedEmail,PasswordHash from AspNetUsers where NormalizedEmail=@NormalizedEmail;", conn);
            comm.Parameters.Add(new MySqlParameter("@NormalizedEmail", MySqlDbType.VarChar, 256)
            {
                Value = normalizedEmail
            });

            using var re = await comm.ExecuteReaderAsync(cancellationToken);
            if (await re.ReadAsync())
            {
                return new ApplicationUser
                {
                    Id = re.GetString(0),
                    UserName = re.GetString(1),
                    NormalizedUserName = re.GetString(2),
                    Email = re.GetString(3),
                    NormalizedEmail = re.GetString(4),
                    PasswordHash = re.GetString(5)
                };
            }

            return null;
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(true);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(true);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            // Just returning an empty list because I don't feel like implementing this. You should get the idea though...
            IList<UserLoginInfo> logins = new List<UserLoginInfo>();
            return Task.FromResult(logins);
        }

        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }

        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using var conn = new MySqlConnection(_connectionWriterString);
            await conn.OpenAsync(cancellationToken);

            using var comm2 = new MySqlCommand("select Id from AspNetRoles where Name=@rolename or NormalizedName=@rolename;", conn);
            comm2.Parameters.Add(new MySqlParameter("@rolename", MySqlDbType.VarChar, 256)
            {
                Value = roleName
            });

            string roleId = (await comm2.ExecuteScalarAsync(cancellationToken)).ToString();

            using var comm1 = new MySqlCommand("select count(*) from AspNetUserRoles where UserId=@userid and RoleId=@roleid;",conn);
            comm1.Parameters.AddRange(new MySqlParameter[]
                {
                    new MySqlParameter("@userid", MySqlDbType.VarChar,256)
                    {
                        Value = user.Id
                    },
                    new MySqlParameter("@roleid", MySqlDbType.VarChar,256)
                    {
                        Value = roleId
                    }
                });

            int userrole = int.Parse((await comm1.ExecuteScalarAsync(cancellationToken)).ToString());
            if (userrole == 0)
            {

                using var comm3 = new MySqlCommand("INSERT INTO AspNetUserRoles (UserId,RoleId) values(@userid, @roleid)", conn);
                comm3.Parameters.AddRange(new MySqlParameter[]
                    {
                    new MySqlParameter("@userid", MySqlDbType.VarChar,256)
                    {
                        Value = user.Id
                    },
                    new MySqlParameter("@roleid", MySqlDbType.VarChar,256)
                    {
                        Value = roleId
                    }
                    });

                int values = await comm3.ExecuteNonQueryAsync(cancellationToken);
                if (values != 1)
                {
                    throw new ArgumentException("Userid or Roleid is wrong");
                }
            }
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using var conn = new MySqlConnection(_connectionReaderString);
            await conn.OpenAsync(cancellationToken);
            using var comm = new MySqlCommand(
                "select r.NormalizedName from AspNetRoles r inner join AspNetUserRoles ur on ur.RoleId = r.Id where ur.UserId = @userid",
                conn);
            comm.Parameters.Add(new MySqlParameter("@userid", MySqlDbType.VarChar, 256)
            {
                Value = user.Id
            });

            using var re = await comm.ExecuteReaderAsync(cancellationToken);

            var coll = new List<string>();
            while (await re.ReadAsync(cancellationToken))
            {
                coll.Add(re.GetString(0));
            }

            return coll;
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            using var conn = new MySqlConnection(_connectionReaderString);
            await conn.OpenAsync(cancellationToken);
            using var comm = new MySqlCommand(
                "select anu.Id, anu.UserName,anu.NormalizedUserName,anu.Email,anu.NormalizedEmail,anu.PasswordHash from AspNetUsers anu "+
                "inner join AspNetUserRoles anur on anur.UserId = anu.id "+
                "inner join AspNetRoles anr on anr.Id = anur.RoleId "+
                "where anr.Name = @rolename or anr.NormalizedName = @rolename;"
                , conn);
            comm.Parameters.Add(new MySqlParameter("@rolename", MySqlDbType.VarChar, 256)
            {
                Value = roleName
            });

            using var re = await comm.ExecuteReaderAsync(cancellationToken);

            var coll = new List<ApplicationUser>();
            while (await re.ReadAsync(cancellationToken))
            {
                coll.Add(new ApplicationUser {
                    Id = re.GetString(0),
                    UserName = re.GetString(1),
                    NormalizedUserName = re.GetString(2),
                    Email = re.GetString(3),
                    NormalizedEmail = re.GetString(4),
                    PasswordHash = re.GetString(5)
                });
            }

            return coll;
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using var conn = new MySqlConnection(_connectionReaderString);
            await conn.OpenAsync(cancellationToken);
            using var comm = new MySqlCommand(
                "select count(*) from AspNetUsers anu " +
                "inner join AspNetUserRoles anur on anur.UserId = anu.id " +
                "inner join AspNetRoles anr on anr.Id = anur.RoleId " +
                "where (anr.Name = @rolename or anr.NormalizedName = @rolename) "+
                "and anu.id=@userid"
                , conn);
            comm.Parameters.AddRange(new MySqlParameter[] {
                    new MySqlParameter("@rolename", MySqlDbType.VarChar, 256)
                    {
                        Value = roleName
                    },
                    new MySqlParameter("@userid", MySqlDbType.VarChar, 256)
                    {
                        Value = user.Id
                    }
                });

            int userCount = int.Parse((await comm.ExecuteScalarAsync(cancellationToken)).ToString());
            return userCount > 0;
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using var conn = new MySqlConnection(_connectionWriterString);
            await conn.OpenAsync(cancellationToken);

            using var comm1 = new MySqlCommand("select Id from AspNetRoles where Name=@rolename or NormalizedName=@rolename;");
            comm1.Parameters.Add(new MySqlParameter("@rolename", MySqlDbType.VarChar, 256)
            {
                Value = roleName
            });

            string roleId = (await comm1.ExecuteScalarAsync(cancellationToken)).ToString();

            using var comm2 = new MySqlCommand("delete from AspNetUserRoles where UserId=@userid and RoleId=@roleid;");
            comm2.Parameters.AddRange(new MySqlParameter[] {
                    new MySqlParameter("@userid", MySqlDbType.VarChar,256)
                    {
                        Value = user.Id
                    },
                    new MySqlParameter("@roleid", MySqlDbType.VarChar,256)
                    {
                        Value = roleId
                    }
                 });

            int values = await comm2.ExecuteNonQueryAsync(cancellationToken);
            if (values != 1)
            {
                throw new ArgumentException("Userid or Roleid is wrong");
            }
        }
    }
}
