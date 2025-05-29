using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UnityEngine.Rendering.DebugUI;
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
        var result = new List<dynamic>();

        try
        {
            _connection.Open();
            Debug.Log("Database connection successful");

            var command = new MySqlCommand(sql, _connection);
            var reader = command.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                result.Add(reader.GetValue(i++));
            }

            reader.Close();
        }

        catch (System.Exception ex)
        {
#if UNITY_EDITOR
            Debug.LogException(ex);
#endif
        }

        finally
        {
            _connection.Close();
        }

        return result.ToArray();
    }

    public dynamic[] SQL(string select, string from, string where, string predicate) => SQL($"SELECT {select} FROM {from} WHERE {where} = '{predicate}';");

    public bool SQLInsert(Dictionary<string, dynamic> values)
    {
        bool success = false;

        try
        {
            _connection.Open();
            Debug.Log("Database connection successful");

            var sql = new StringBuilder($"INSERT INTO {DatabaseName} (");
            for (int i = 0; i < values.Count; i++)
            {
                sql.Append($"{values.ElementAt(i).Key}");
                if (i < values.Count - 1 ) sql.Append(", ");
                else sql.Append(") ");
            }

            sql.Append("VALUES (");
            for (int i = 0; i < values.Count; i++)
            {
                sql.Append($"'{values.ElementAt(i).Value}'");
                if (i < values.Count - 1 ) sql.Append(", ");
                else sql.Append(") ");
            }

            sql.Append(");");

            var command = new MySqlCommand(sql.ToString(), _connection);

            success = true;
        }
        catch (System.Exception ex)
        {
#if UNITY_EDITOR
            Debug.LogException(ex);
#endif
        }
        finally
        {
            _connection.Close();
        }

        return success;
    }
}
