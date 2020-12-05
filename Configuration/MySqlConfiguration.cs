namespace Configuration
{
	public class MySqlConfiguration
	{
		public MySqlConnectionInfo MySqlConnection = new MySqlConnectionInfo();
	}

	public class MySqlConnectionInfo
	{
		public string Server { get; set; } = null!;
		public string DatabaseName { get; set; } = null!;
		public string Username { get; set; } = null!;
		public string Password { get; set; } = null!;
	}
}
