using MySql.Data.MySqlClient;
using System.IO;
using System;
using Debug = UnityEngine.Debug;
using Unity.VisualScripting;
using UnityEngine;

public class DatabaseHandler
{
#if UNITY_EDITOR
    private readonly string CredentialsPath = Path.Combine(Application.dataPath, "DBCredentials.json");
#else
    private const string CredentialsPath = "";
#endif
    private DBCredentials _credentials;

    private readonly MySqlConnection _connection;

    public DatabaseHandler() => _connection = new MySqlConnection(LoadCredentials().GetConnectionString());
    ~DatabaseHandler() => _connection.Close();

    public DBCredentials LoadCredentials()
    {
        if (!File.Exists(CredentialsPath))
        {
            GeneratePathFile();
            Debug.Log($"FEHLER: Credentials-Datei fehlt. Öffne DBCredentials.json und fülle die Felder aus. Datei befindet sich im Pfad {CredentialsPath}");
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
