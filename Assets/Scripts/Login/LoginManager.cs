using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private TMP_Text _messageDisplay;

    private DatabaseHandler _dbHandler;

    private const int MinPasswordLength = 5;

    private void Awake()
    {
        try
        {
            _dbHandler = new();
        }
        catch (System.Exception ex)
        {
            _messageDisplay.text = "Connection to database could not be established.";
#if UNITY_EDITOR
            Debug.LogException(ex);
#endif
        }
    }

    public void TrySubmitLogin()
    {
        string username = _usernameInput.text;
        string password = _passwordInput.text;

        //var passwordResult = _dbHandler.SQL($"SELECT password FROM user WHERE username = '{username}';");
        var passwordResult = _dbHandler.SQL(select: "password", from: "user", where: "username", predicate: username);

        // User could not be found
        if (passwordResult?.Length == 0)
        {
            _messageDisplay.text = "User could not be found";
            _usernameInput.image.color = Color.red;
            return;
        }

        // Edge-case error
        if (passwordResult.Length > 1 || passwordResult[0] is not string)
        {
            _messageDisplay.text = "Critical error. I have no idea what went wrong";
            return;
        }

        // Wrong password
        if (!passwordResult[0].Equals(password))
        {
            _messageDisplay.text = "Wrong password!";
            _passwordInput.image.color = Color.red;
            return;
        }

        var userData = GetUser(username);
        CurrentUser.SetUser(userData);
    }

    public void TryRegisterNewUser()
    {
        string username = _usernameInput.text;
        string password = _passwordInput.text;

        if (username.Length == 0)
        {
            _messageDisplay.text = "Enter a username!";
            return;
        }

        if (password.Length < MinPasswordLength)
        {
            _messageDisplay.text = "Password needs to be at least 5 characters long!";
            return;
        }

        if (_dbHandler.SQL(select: "username", from: "user", where: username, predicate: username).Length == 0)
        {
            _messageDisplay.text = "User with this name already exists!";
            return;
        }

        var newUserData = new Dictionary<string, object>()
        {
            {"username", username},
            {"password", password},
            {"streak", 0},
            {"XP", 0}
        };

        if (!_dbHandler.SQLInsert(newUserData))
        {
            _messageDisplay.text = "Could not register user.";
            return;
        }

        TrySubmitLogin();
    }

    public UserData GetUser(string username)
    {
        var userDataRaw = _dbHandler.SQL($"SELECT * FROM user WHERE username = '{username}';");
        return new UserData(userDataRaw[0], userDataRaw[1], userDataRaw[2], userDataRaw[3], userDataRaw[4]);
    }
}
