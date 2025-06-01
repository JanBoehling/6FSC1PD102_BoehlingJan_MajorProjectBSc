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

    private const int MinPasswordLength = 5;

    

    private async void Awake()
    {
        if (!(await DB.TestConnection()).Equals("Connection successful!"))
        {
            _messageDisplay.text = ErrorMessages.ConnectionToDBFailedError;

            _loginButton.interactable = false;
            _registerButton.interactable = false;
        }
    }

    public async void TrySubmitLogin()
    {
        string username = _usernameInput.text;
        string password = _passwordInput.text;

        //var passwordResult = _dbHandler.SQL($"SELECT password FROM user WHERE username = '{username}';");
        var passwordResult = await DB.Select(select: "password", from: "user", where: "username", predicate: username);

        // User could not be found
        if (string.IsNullOrEmpty(passwordResult))
        {
            _messageDisplay.text = ErrorMessages.UserNotFoundError;
            _usernameInput.image.color = Color.red;
            return;
        }

        // Wrong password
        if (!passwordResult.Equals(password))
        {
            _messageDisplay.text = ErrorMessages.WrongPasswordError;
            _passwordInput.image.color = Color.red;
            return;
        }

        var userData = GetUser(username);
        CurrentUser.SetUser(userData);
    }

    public async void TryRegisterNewUser()
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

        if (string.IsNullOrEmpty(await DB.Select(select: "username", from: "user", where: username, predicate: username)))
        {
            _messageDisplay.text = ErrorMessages.UserAlreadyExistsError;
            return;
        }

        // Insert new user to db
        await DB.Insert(username, password, 0, 0);

        // Add every progression data to user
        AddProgressionData(username);

        TrySubmitLogin();
    }

    private async void AddProgressionData(string username)
    {
        string userID = await DB.Select(select: "userID", from: "UserData", where: "username", predicate: username);

        if (userID.Length == 0)
        {
            string errorMessage = ErrorMessages.UserIDNotFoundError.Replace("%s", userID.ToString());
            Debug.LogError(errorMessage);
            return;
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
            await DB.Insert(Table.UnitProgress, (uint)item["unitLink"], (byte)item["isCompleted"], (uint)item["userID"]);
        }
        foreach (var item in assignmentData)
        {
            await DB.Insert(Table.AssignmentProgress, (uint)item["assignmentLink"], (byte)item["isCompleted"], (uint)item["userID"]);
        }

        return;
    }

    public UserData GetUser(string username)
    {
        var userDataRaw = DB.Query($"SELECT * FROM UserData WHERE username = '{username}';");
        return null;
        //return new UserData(userDataRaw[0], userDataRaw[1], userDataRaw[2], userDataRaw[3], userDataRaw[4]);
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

    public override async void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (GUILayout.Button("Test MySQL Connection")) Debug.Log(await DB.TestConnection());
    }
}
#endif
