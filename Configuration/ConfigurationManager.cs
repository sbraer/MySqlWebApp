using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Configuration
{
	public interface IConfigurationManager
	{
		string GetIdentifier();
		string GetConnectionWriterString();
		string GetConnectionReaderString();
	}

	public class ConfigurationManager : IConfigurationManager
	{
		private readonly string _identifier;
		private readonly MySqlConnectionInfo _mySqlConnectionWriter;
		private readonly MySqlConnectionInfo _mySqlConnectionReader;
		private string? _connectionWriterString;
		private string? _connectionReaderString;

		public ConfigurationManager(IConfiguration configuration)
		{
			_identifier = Guid.NewGuid().ToString();
			_mySqlConnectionWriter = configuration.GetSection("MySqlConnectionWriter").Get<MySqlConnectionInfo>();
			_mySqlConnectionReader = configuration.GetSection("MySqlConnectionReader").Get<MySqlConnectionInfo>();

			if (_mySqlConnectionReader == null)
			{
				_mySqlConnectionReader = _mySqlConnectionWriter;
			}
		}

		public string GetIdentifier() => _identifier;
		public string GetConnectionWriterString()
		{
			if (string.IsNullOrEmpty(_connectionWriterString))
			{
				string serverName = GetEnviromentVariable(_mySqlConnectionWriter.Server, "MySqlServerName", false);
				string databaseName = GetEnviromentVariable(_mySqlConnectionWriter.DatabaseName, "MySqlDatabaseName", false);
				string username = GetEnviromentVariable(_mySqlConnectionWriter.Username, "MySqlUsername", true);
				string password = GetEnviromentVariable(_mySqlConnectionWriter.Password, "MySqlPassword", true);
				_connectionWriterString = $"Server={serverName};Database={databaseName};Uid={username};Pwd={password};";
			}

			return _connectionWriterString;
		}

		public string GetConnectionReaderString()
		{
			if (string.IsNullOrEmpty(_connectionReaderString))
			{
				string serverName = GetEnviromentVariable(_mySqlConnectionReader.Server, "MySqlServerNameReader", false);
				string databaseName = GetEnviromentVariable(_mySqlConnectionReader.DatabaseName, "MySqlDatabaseNameReader", false);
				string username = GetEnviromentVariable(_mySqlConnectionReader.Username, "MySqlUsernameReader", true);
				string password = GetEnviromentVariable(_mySqlConnectionReader.Password, "MySqlPasswordReader", true);
				_connectionReaderString = $"Server={serverName};Database={databaseName};Uid={username};Pwd={password};";
			}

			return _connectionReaderString;
		}

		private string GetEnviromentVariable(string defaultValue, string environmentVariable, bool searchFile)
		{
			if (searchFile)
			{
				string? fileName = Environment.GetEnvironmentVariable(environmentVariable + "File");
				if (fileName != null)
				{
					if (File.Exists(fileName))
					{
						return File.ReadAllText(fileName).Trim();
					}
				}
			}

			if (Environment.GetEnvironmentVariable(environmentVariable) != null)
			{
				string? value = Environment.GetEnvironmentVariable(environmentVariable);
				if (value != null)
				{
					return value;
				}
			}

			return defaultValue;
		}
	}
}
