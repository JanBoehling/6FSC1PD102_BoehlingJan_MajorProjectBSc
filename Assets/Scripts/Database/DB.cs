using System;
using System.Net.Http;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

/// <summary>
/// Enables communication not with the Deutsche Bahn, but with the Database via PHP scripts located on the server.<br/>
/// This class uses the UnityWebRequest class to call PHP scripts on the same server that the database lies on. These PHP scripts execute SQL Queries that communicate directly with the database.
/// </summary>
public static class DB
{
    // Base request url
    private const string Url = "http://6fsc1pd102-boehlingjan-majorprojectbsc.de/";

    // PHP File names (Can be found under Assets/PHP)
    private const string PhpTest = "testConnection.php";
    private const string PhpSelect = "SELECT.php";
    private const string PhpSelectWhere = "SELECTWHERE.php";
    private const string PhpInsertUserData = "UserDataINSERT.php";
    private const string PhpInsertUnitProgress = "UnitProgressINSERT.php";
    private const string PhpInsertAssignmentProgress = "AssignmentProgressINSERT.php";
    private const string PhpQuery = "QUERY.php";
    private const string PhpUpdate = "UPDATE.php";

    /// <summary>
    /// Tests connection
    /// </summary>
    /// <returns>The success message of the connection to the database</returns>
    public static async Task<string[]> TestConnection()
    {
        var requestURL = $"{Url}{PhpTest}";
        return await WebRequest(HttpMethod.Get, requestURL);
    }

    /// <summary>
    /// Executes a SQL Query on the Database
    /// </summary>
    /// <param name="sqlQuery">The SQL Command string</param>
    /// <returns>The raw result from the SQL query as a string</returns>
    public static async Task<string[]> Query(string sqlQuery)
    {
        var requestURL = $"{Url}{PhpQuery}?sql={sqlQuery}";
        return await WebRequest(HttpMethod.Post, requestURL);
    }

    /// <summary>
    /// Retrieves value from DB. Syntax based of SQL SELECT
    /// </summary>
    /// <param name="select">DB Column that should be retrieved</param>
    /// <param name="from">Table name</param>
    /// <param name="where">Constraint</param>
    /// <param name="predicate">Constraint-value</param>
    /// <returns>The raw result from the SQL query as a string</returns>
    public static async Task<string[]> Select(string select, string from, string where, string predicate)
    {
        var requestURL = $"{Url}{PhpSelectWhere}?select={select}&from={from}&where={where}&predicate={predicate}";
        return await WebRequest(HttpMethod.Post, requestURL);
    }

    /// <summary>
    /// Retrieves value from DB. Syntax based of SQL SELECT
    /// </summary>
    /// <param name="select">DB Column that should be retrieved</param>
    /// <param name="from">Table name</param>
    /// <returns>The raw result from the SQL query as a string</returns>
    public static async Task<string[]> Select(string select, string from)
    {
        var requestURL = $"{Url}{PhpSelect}?select={select}&from={from}";
        return await WebRequest(HttpMethod.Post, requestURL);
    }

    /// <summary>
    /// Creates a new record inside the Database
    /// </summary>
    /// <param name="username">The name of the new user</param>
    /// <param name="password">The password of the new user</param>
    /// <param name="streak">The current streak of the user. Should always be 0 at account creation</param>
    /// <param name="XP">The current XP of the user. Should always be 0 at account creation</param>
    /// <returns>The raw result from the SQL query as a string</returns>
    public static async Task<string[]> Insert(string username, string password, uint streak, uint XP)
    {
        var requestURL = $"{Url}{PhpInsertUserData}?username={username}&password={password}&streak={streak}&XP={XP}";
        return await WebRequest(HttpMethod.Post, requestURL);
    }

    /// <summary>
    /// Creates a new record inside the Database
    /// </summary>
    /// <param name="username">The name of the new user</param>
    /// <param name="password">The password of the new user</param>
    /// <param name="streak">The current streak of the user. Should always be 0 at account creation</param>
    /// <param name="XP">The current XP of the user. Should always be 0 at account creation</param>
    /// <returns>The raw result from the SQL query as a string</returns>
    public static async Task<string[]> Insert(Table table, uint link, uint isCompleted, uint userID)
    {
        if (table == Table.UserData)
        {
            Debug.LogError("Wrong Insert method used. Cannot insert progress data into UserData Table!");
            return null;
        }

        string insertPHP = table switch
        {
            Table.UnitProgress => PhpInsertUnitProgress,
            Table.AssignmentProgress => PhpInsertAssignmentProgress,
            Table.UserData => PhpInsertUserData, // this case should never be reached
            _ => ""
        };

        string requestURL = $"{Url}{insertPHP}?link={link}&isCompleted={isCompleted}&userID={userID}";
        return await WebRequest(HttpMethod.Post, requestURL);
    }

    public static async Task<string[]> Update(string tableName, string column, string value, string where = "UserID", string predicate = null)
    {
        predicate ??= CurrentUser.UserID.ToString();

        string requestURL = $"{Url}{PhpUpdate}?table={tableName}&set={column}={value}&where={where}&predicate={predicate}";
        return await WebRequest(HttpMethod.Post, requestURL);
    }

    public static async Task<string[]> Update(string tableName, System.Collections.Generic.KeyValuePair<string, string>[] values, string where = "UserID", string predicate = null)
    {
        predicate ??= CurrentUser.UserID.ToString();

        var setStringBuilder = new System.Text.StringBuilder();
        foreach (var item in values)
        {
            setStringBuilder.Append($"{item.Key} = {item.Value},");
        }
        setStringBuilder.Remove(setStringBuilder.Length - 1, 1);

        string requestURL = $"{Url}{PhpUpdate}?table={tableName}&set={setStringBuilder}&where={where}&predicate={predicate}";
        return await WebRequest(HttpMethod.Post, requestURL);
    }

    /// <summary>
    /// Creates a web request using the systems' HttpClient
    /// </summary>
    /// <param name="method">The method of request</param>
    /// <param name="requestURL">The request url. Combination of base url plus php file plus various parameters</param>
    /// <returns>The raw result from the SQL query as a string or null if not successful</returns>
    private static async Task<string[]> WebRequest(HttpMethod method, string requestURL)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(method, requestURL);

        var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError(body);
            return null;
        }

        return body.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }
}

public enum Table
{
    UserData,
    UnitProgress,
    AssignmentProgress
}
