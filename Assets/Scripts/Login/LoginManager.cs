using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

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
    [Space]
    [SerializeField] private float CheckConnectionDelay = 2f;
    [SerializeField] private uint MinPasswordLength = 5;

    private float _checkConnectionTimer;

    private void Start()
    {
        _checkConnectionTimer = CheckConnectionDelay;
    }

    private void Update()
    {
        _checkConnectionTimer += Time.deltaTime;

        if (_checkConnectionTimer < CheckConnectionDelay) return;

        TestConnection();

        _checkConnectionTimer = 0f;
    }

    private async void TestConnection()
    {
        var testConnection = await DB.TestConnection();

        if (Application.internetReachability == NetworkReachability.NotReachable || testConnection is null || !testConnection[0].Equals("0"))
        {
            _messageDisplay.text = ErrorMessages.ConnectionToDBFailedError;

            _loginButton.interactable = false;
            _registerButton.interactable = false;
        }
        else
        {
            _messageDisplay.text = "";
            _loginButton.interactable = true;
            _registerButton.interactable = true;
        }
    }

    public async void TrySubmitLogin(string username, string password)
    {
        _messageDisplay.text = "Logging in...";
        var passwordResult = await DB.Select(select: "password", from: "UserData", where: "username", predicate: username);
        _messageDisplay.text = "Log in successful!";

        // User could not be found
        if (string.IsNullOrEmpty(passwordResult[0]))
        {
            _messageDisplay.text = ErrorMessages.UserNotFoundError;
            _usernameInput.image.color = Color.red;
            return;
        }

        // Wrong password
        if (!passwordResult[0].Equals(password))
        {
            _messageDisplay.text = ErrorMessages.WrongPasswordError;
            _passwordInput.image.color = Color.red;
            return;
        }

        var userData = await GetUser(username);
        CurrentUser.SetUser(userData);

        CompletionTracker.Instance.DownloadCompletionData();

        Debug.Log($"<color=green>Successfully logged int user with ID {userData.UserID}</color>");
        SceneManager.LoadScene(1);
    }

    public void TrySubmitLogin() => TrySubmitLogin(_usernameInput.text, _passwordInput.text);

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

        var checkForDuplicateUsername = await DB.Select(select: "username", from: "UserData", where: "username", predicate: username);
        if (checkForDuplicateUsername.Length > 0)
        {
            _messageDisplay.text = ErrorMessages.UserAlreadyExistsError;
            return;
        }

        // Insert new user to db
        var insert = await DB.Insert(username, password, 0, 0);

        // Add every progression data to user
        bool success = await AddProgressionData(username);
        if (!success)
        {
            Debug.LogError("Could not add progression data!");
            return;
        }

        TrySubmitLogin();
    }

    private async Task<bool> AddProgressionData(string username)
    {
        var userIDRaw = await DB.Select(select: "userID", from: "UserData", where: "username", predicate: username);

        if (userIDRaw.Length == 0)
        {
            string errorMessage = ErrorMessages.UserIDNotFoundError.Replace("%s", username);
            Debug.LogError(errorMessage);
            return false;
        }

        uint userID = uint.Parse(userIDRaw[0]);

        var unitData = new uint[CompletionTracker.Instance.Units.Length][];
        for (uint i = 0; i < CompletionTracker.Instance.Units.Length; i++)
        {
            unitData[i] = new uint[]
            {
                i, // unitLink
                0, // isCompleted
                userID
            };
        }

        var assignmentData = new uint[CompletionTracker.Instance.Assignments.Length][];
        for (uint i = 0; i < CompletionTracker.Instance.Assignments.Length; i++)
        {
            assignmentData[i] = new uint[]
            {
                i, // assignmentLink
                0, // isCompleted
                userID
            };
        }

        foreach (var item in unitData)
        {
            await DB.Insert(Table.UnitProgress, item[0], item[1], item[2]);
        }
        foreach (var item in assignmentData)
        {
            await DB.Insert(Table.AssignmentProgress, item[0], item[1], item[2]);
        }

        return true;
    }

    public async Task<UserData> GetUser(string username)
    {
        const int coulumCount = 5;

        var result = await DB.Select(select: "*", from: "UserData", where: "username", predicate: username);

        if (result.Length > coulumCount)
        {
            Debug.LogError($"Multiple entires in DB found for {username}!");
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

#if UNITY_EDITOR
[CustomEditor(typeof(LoginManager))]
public class LoginManagerEditor : Editor
{
    private LoginManager _loginManager;

    private bool _doAutoLogin;

    private void OnEnable()
    {
        _loginManager = (LoginManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying) return;

        EditorGUILayout.Space();

        if (GUILayout.Button("Login as admin"))
        {
            _loginManager.TrySubmitLogin("admin", "t8734qzp920ßtvhrtbui23op");
        }
    }
}
#endif
