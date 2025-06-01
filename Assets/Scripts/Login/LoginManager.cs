using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

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

        var userData = await GetUser(username);
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

    public async Task<UserData> GetUser(string username)
    {
        const int coulumCount = 5;

        string userDataRaw = await DB.Select(select: "*", from: "UserData", where: "username", predicate: username);
        var result = userDataRaw.Split('\n');

        if (result.Length > coulumCount)
        {
            Debug.LogError("Multiple entires in DB found for {username}!");
            return null;
        }
        else if (result.Length < coulumCount)
        {
            Debug.LogError("Could not fetch complete data!");
            return null;
        }
        else return new UserData(uint.Parse(result[0]), result[1], result[2], uint.Parse(result[3]), uint.Parse(result[4]));
    }
}
