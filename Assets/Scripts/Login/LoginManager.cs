using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private TMP_InputField _passwordInput;

    private DatabaseHandler _dbHandler;

    private void Awake()
    {
        _dbHandler = new();
    }

    public void TrySubmitLogin()
    {
        string username = _usernameInput.text;
        string password = _passwordInput.text;

        var passwordResult = _dbHandler.SQL($"SELECT password FROM user WHERE username = '{username}';");

        // User could not be found
        if (passwordResult.Length == 0)
        {
            Debug.LogError("User could not be found");
            _usernameInput.image.color = Color.red;
            return;
        }

        // Edge-case error
        if (passwordResult.Length > 1 || passwordResult[0] is not string)
        {
            Debug.Log("Critical error. I have no idea what went wrong");
            return;
        }

        // Wrong password
        if (!passwordResult[0].Equals(password))
        {
            Debug.Log("Wrong password");
            _passwordInput.image.color = Color.red;
            return;
        }

        var userData = GetUser(username);
        CurrentUser.SetUser(userData);
    }

    public UserData GetUser(string username)
    {
        var userDataRaw = _dbHandler.SQL($"SELECT * FROM user WHERE username = '{username}';");
        return new UserData(userDataRaw[0], userDataRaw[1], userDataRaw[2], userDataRaw[3], userDataRaw[4]);
    }
}
