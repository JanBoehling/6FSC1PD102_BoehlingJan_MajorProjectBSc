using MySql.Data.MySqlClient;
using Debug = UnityEngine.Debug;

public class DatabaseHandler
{
#if UNITY_EDITOR
    private const string ServerAddress = "localhost";
    private const string User = "root";
    private const string DatabaseName = "";
    private const int Port = -1;
    private const string Password = "";
#else
    private const string ServerAddress = "";
    private const string User = "";
    private const string DatabaseName = "";
    private const int Port = -1;
    private const string Password = ""; //I know, this is very dangerous, but I trust you :)
#endif

    private readonly MySqlConnection _connection;

    public DatabaseHandler() => _connection = new MySqlConnection($"server={ServerAddress};user={User};database={DatabaseName};port={Port};password={Password}");
    ~DatabaseHandler() => _connection.Close();

    /// <summary>
    /// Tries to open up a connection to the database and queries for the given string.
    /// </summary>
    /// <param name="sql">The SQL Query</param>
    /// <returns>The result from the database</returns>
    public dynamic[] SQL(string sql)
    {
        try
        {
            _connection.Open();
            Debug.Log("Database connection successful");

            var command = new MySqlCommand(sql, _connection);
            var reader = command.ExecuteReader();

            var result = new System.Collections.Generic.List<dynamic>();

            int i = 0;
            while (reader.Read())
            {
                result.Add(reader.GetValue(i++));
            }

            reader.Close();
            return result.ToArray();
        }

        catch (System.Exception ex)
        {
            Debug.LogException(ex);

            return null;
        }

        finally
        {
            _connection.Close();
        }
    }
}
