using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DatabaseHandler
{
    private readonly string CredentialsPath = Path.Combine(Application.dataPath, "DBCredentials.json");
    private DBCredentials _credentials;

    private readonly MySqlConnection _connection;

    public DatabaseHandler() => _connection = new MySqlConnection(LoadCredentials().GetConnectionString());
    ~DatabaseHandler() => _connection.Close();

    public DBCredentials LoadCredentials()
    {
        if (!File.Exists(CredentialsPath))
        {
            GeneratePathFile();
            Debug.Log($"FEHLER: Credentials-Datei fehlt. ?ffne DBCredentials.json und f?lle die Felder aus. Datei befindet sich im Pfad {CredentialsPath}");
            return _credentials;
        }

        using var reader = new StreamReader(CredentialsPath);
        string json = reader.ReadToEnd();

        _credentials = JsonUtility.FromJson<DBCredentials>(json);

        return _credentials;
    }

    public void GeneratePathFile()
    {
        string json = JsonUtility.ToJson(new DBCredentials(), true);
        File.Create(CredentialsPath).Close();
        using var stream = new StreamWriter(CredentialsPath);
        stream.Write(json);
        stream.Close();
    }

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

    public bool SQLInsert(Dictionary<string, dynamic> values, string tableName = "UserData")
    {
        bool success = false;

        try
        {
            _connection.Open();
            Debug.Log("Database connection successful");

            var sql = new StringBuilder($"INSERT INTO {tableName} (");
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
