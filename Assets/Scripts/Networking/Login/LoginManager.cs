using System;
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

    private void TestConnection()
    {
        var callback = new Action<string[]>((testConnection) =>
        {
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
        });

        DB.Instance.TestConnection(callback);
    }

    public void TrySubmitLogin(string username, string password)
    {
        _messageDisplay.text = "Logging in...";

        var passwordCorrectCallback = new Action<string[]>((passwordResult) =>
        {
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

            var getUserCallback = new Action<UserData>((userData) =>
            {
                CurrentUser.SetUser(userData);

                UnitAndAssignmentManager.Instance.DownloadCompletionData();

                Debug.Log($"<color=green>Successfully logged int user with ID {userData.UserID}</color>");
                SceneManager.LoadScene(1);
            });

            GetUser(getUserCallback, username);
            
        });

        DB.Instance.Select(passwordCorrectCallback, select: "password", from: "UserData", where: "username", predicate: username);
    }

    public void TrySubmitLogin() => TrySubmitLogin(_usernameInput.text, _passwordInput.text);

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

        var checkForDuplicateUsernameCallback = new Action<string[]>((checkForDuplicateUsername) =>
        {
            if (checkForDuplicateUsername.Length > 0)
            {
                _messageDisplay.text = ErrorMessages.UserAlreadyExistsError;
                return;
            }

            // Add every progression data to user
            var addProgressionDataCallback = new Action<string[]>((insert) =>
            {
                var addProgressionDataSuccessCallback = new Action<bool>((success) =>
                {
                    if (!success)
                    {
                        Debug.LogError("Could not add progression data!");
                        return;
                    }

                    TrySubmitLogin();
                });

                AddProgressionData(addProgressionDataSuccessCallback, username);
            });

            // Insert new user to db
            DB.Instance.Insert(addProgressionDataCallback, username, password, 0, 0);
        });

        DB.Instance.Select(checkForDuplicateUsernameCallback, select: "username", from: "UserData", where: "username", predicate: username);
        
    }

    private void AddProgressionData(Action<bool> callback, string username)
    {
        var getUserIDCallback = new Action<string[]>((userIDRaw) =>
        {
            if (userIDRaw.Length == 0)
            {
                string errorMessage = ErrorMessages.UserIDNotFoundError.Replace("%s", username);
                Debug.LogError(errorMessage);
                callback?.Invoke(false);
                return;
            }

            uint userID = uint.Parse(userIDRaw[0]);

            var unitData = new uint[UnitAndAssignmentManager.Instance.Units.Length][];
            for (uint i = 0; i < UnitAndAssignmentManager.Instance.Units.Length; i++)
            {
                unitData[i] = new uint[]
                {
                i, // unitLink
                0, // isCompleted
                userID
                };
            }

            var assignmentData = new uint[UnitAndAssignmentManager.Instance.Assignments.Length][];
            for (uint i = 0; i < UnitAndAssignmentManager.Instance.Assignments.Length; i++)
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
                DB.Instance.Insert(null, Table.UnitProgress, item[0], item[1], item[2]);
            }
            foreach (var item in assignmentData)
            {
                DB.Instance.Insert(null, Table.AssignmentProgress, item[0], item[1], item[2]);
            }
        });
        
        DB.Instance.Select(getUserIDCallback, select: "userID", from: "UserData", where: "username", predicate: username);
        callback?.Invoke(true);
    }

    public void GetUser(Action<UserData> callback, string username)
    {
        const int coulumCount = 5;

        var getUserCallback = new Action<string[]>((result) =>
        {
            if (result.Length -1 > coulumCount)//-1 because of error code being passed as last element in array
            {
                Debug.LogError($"Multiple entires in DB found for {username}!");
                callback?.Invoke(null);
                return;
            }
            else if (result.Length +1 < coulumCount)//+1 because of error code being passed as last element in array
            {
                Debug.LogError("Could not fetch complete data!");
                callback?.Invoke(null);
                return;
            }
            else
            {
                callback?.Invoke(new UserData(uint.Parse(result[0]), result[1], result[2], uint.Parse(result[3]), uint.Parse(result[4])));
                return;
            }
        });

        DB.Instance.Select(getUserCallback, select: "*", from: "UserData", where: "username", predicate: username);
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
