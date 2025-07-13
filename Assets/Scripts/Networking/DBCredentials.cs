[System.Serializable]
public struct DBCredentials
{
    public string Server;
    public string UserID;
    public string Database;
    public uint Port;
    public string Password;

    public DBCredentials(string server = "database-5017942646.webspace-host.com", string userID = "dbu3329914", string database = "dbs14278174", uint port = 3306, string password = "")
    {
        Server = server;
        UserID = userID;
        Database = database;
        Port = port;
        Password = password;
    }

    public string GetConnectionString() => new MySql.Data.MySqlClient.MySqlConnectionStringBuilder()
    {
        Server = Server,
        UserID = UserID,
        Database = Database,
        Port = Port,
        Password = Password
    }.ConnectionString;
}
