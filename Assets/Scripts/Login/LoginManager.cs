using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _registerButton;
    [SerializeField] private TMP_Text _messageDisplay;

    private DB _dbHandler;

    private const int MinPasswordLength = 5;

    

    private void Awake()
    {
        try
        {
            _dbHandler = new();
        }
        catch (System.Exception ex)
        {
            _messageDisplay.text = ErrorMessages.ConnectionToDBFailedError;

            _loginButton.interactable = false;
            _registerButton.interactable = false;
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
            _messageDisplay.text = ErrorMessages.UserNotFoundError;
            _usernameInput.image.color = Color.red;
            return;
        }

        // Edge-case error
        if (passwordResult.Length > 1 || passwordResult[0] is not string)
        {
            _messageDisplay.text = ErrorMessages.GenericError;
            return;
        }

        // Wrong password
        if (!passwordResult[0].Equals(password))
        {
            _messageDisplay.text = ErrorMessages.WrongPasswordError;
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
            _messageDisplay.text = ErrorMessages.UsernamePromptEmptyError;
            return;
        }

        if (password.Length < MinPasswordLength)
        {
            _messageDisplay.text = ErrorMessages.PasswordPromptEmptyError;
            return;
        }

        if (_dbHandler.SQL(select: "username", from: "user", where: username, predicate: username).Length == 0)
        {
            _messageDisplay.text = ErrorMessages.UserAlreadyExistsError;
            return;
        }

        var newUserData = new Dictionary<string, object>()
        {
            {"username", username},
            {"password", password},
            {"streak", 0},
            {"XP", 0}
        };

        // Insert new user to db
        if (!_dbHandler.SQLInsert(newUserData))
        {
            _messageDisplay.text = ErrorMessages.RegisterUserFailedError;
            return;
        }

        // Add every progression data to user
        if (!AddProgressionData(username))
        {
            _messageDisplay.text = ErrorMessages.AddProgressionDataFailedError;
            return;
        }

        TrySubmitLogin();
    }

    private bool AddProgressionData(string username)
    {
        var userID = _dbHandler.SQL(select: "userID", from: "UserData", where: "username", predicate: username);

        if (userID.Length == 0)
        {
            string errorMessage = ErrorMessages.UserIDNotFoundError.Replace("%s", userID.ToString());
            Debug.LogError(errorMessage);
            return false;
        }

        var unitData = new Dictionary<string, object>[CompletionTracker.Instance.Units.Length];
        for (int i = 0; i < unitData.Length; i++)
        {
            unitData[i] = new()
            {
                {"unitLink", i},
                {"isCompleted", 0},
                {"userID", userID[0]}
            };
        }

        var assignmentData = new Dictionary<string, object>[CompletionTracker.Instance.Assignments.Length];
        for (int i = 0; i < CompletionTracker.Instance.Assignments.Length; i++)
        {
            unitData[i] = new()
            {
                {"assignmentLink", i},
                {"isCompleted", 0},
                {"userID", userID[0]}
            };
        }

        foreach (var item in unitData)
        {
            if (_dbHandler.SQLInsert(item, "UnitProgress")) continue;

            Debug.LogError($"Could not insert unit with link number {item["unitLink"]}");
            return false;
        }
        foreach (var item in assignmentData)
        {
            if (_dbHandler.SQLInsert(item, "AssignmentProgress")) continue;

            Debug.LogError($"Could not insert assignment with link number {item["assignmentLink"]}");
            return false;
        }

        return true;
    }

    public UserData GetUser(string username)
    {
        var userDataRaw = _dbHandler.SQL($"SELECT * FROM UserData WHERE username = '{username}';");
        return new UserData(userDataRaw[0], userDataRaw[1], userDataRaw[2], userDataRaw[3], userDataRaw[4]);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LoginManager))]
public class LoginManagerEditor : Editor
{
    private LoginManager _loginManager;

    private void OnEnable()
    {
        _loginManager = (LoginManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (GUILayout.Button("Test MySQL Connection")) new DB().TestConnection();
    }
}
#endif
