using System.Net.Http;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

/// <summary>
/// Enables communication not with the Deutsche Bahn, but with the Database via PHP scripts located on the server.<br/>
/// This class uses the UnityWebRequest class to call PHP scripts on the same server that the database lies on. These PHP scripts execute SQL Queries that communicate directly with the database.
/// </summary>
public static class DB
{
    private const string Url = "";

    // PHP File names
    private const string PhpSelect = "UserDataSELECT.php";

    //public static async Task<(HttpResponseMessage response, string body)?> Query(string sqlQuery)
    //{
    //    
    //}

    public static async Task<(HttpResponseMessage response, string body)?> Select(string select, string from, string where, string predicate)
    {
        var requestURL = $"{Url}/{PhpSelect}?select={select}&from={from}&where={where}&predicate={predicate}";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, requestURL);

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

        return (response, body);
    }
}
