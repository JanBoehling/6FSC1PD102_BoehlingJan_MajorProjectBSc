using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private GameObject _loadingImage;
    [Space]
    [SerializeField] private Image _titleImage;
    [SerializeField] private Sprite _celloDefaultSprite;
    [SerializeField] private Sprite _celloSadSprite;
    [SerializeField] private Sprite _celloVerySadSprite;
    [SerializeField] private Sprite _celloExtremelySadSprite;
    [Space]
    [SerializeField] private float CheckConnectionDelay = 2f;
    [SerializeField] private uint MinPasswordLength = 5;

    private float _checkConnectionTimer = 0f;
    private bool _hasCollectionBeenLost = false;
    private bool _doTestConnection = true;

    private void Start()
    {
        _checkConnectionTimer = CheckConnectionDelay;
    }

    private void Update()
    {
        _checkConnectionTimer += Time.deltaTime;

        if (_doTestConnection && _checkConnectionTimer < CheckConnectionDelay) return;

        TestConnection();

        _checkConnectionTimer = 0f;
    }

    private void SetTitleImage(Sprite sprite = null)
    {
        if (sprite) _titleImage.sprite = sprite;
        else _titleImage.sprite = _celloDefaultSprite;
    }

    private void SetMessage(string msg = "")
    {
        _messageDisplay.text = msg;

        if (string.IsNullOrWhiteSpace(msg) && _loadingImage) _loadingImage.SetActive(true);
        else if (_loadingImage) _loadingImage.SetActive(false);
    }

    private void TestConnection()
    {
        DB.Instance.TestConnection((testConnection) =>
        {
            if (Application.internetReachability == NetworkReachability.NotReachable || testConnection is null || !testConnection[0].Equals("0"))
            {
                SetMessage(ErrorMessages.ConnectionToDBFailedError);
                SetTitleImage(_celloExtremelySadSprite);

                _loginButton.interactable = false;
                _registerButton.interactable = false;

                _hasCollectionBeenLost = true;
            }
            else if (_hasCollectionBeenLost)
            {
                SetMessage();
                SetTitleImage();

                _loginButton.interactable = true;
                _registerButton.interactable = true;
                _hasCollectionBeenLost = false;
            }
        });
    }

    // Used for submit event
    public void TrySubmitLogin(string password) => TrySubmitLogin(_usernameInput.text, password);

    public void TrySubmitLogin(string username, string password)
    {
        SetMessage();
        _doTestConnection = false;

        DB.Instance.Select((passwordResult) =>
        {
            // User could not be found
            if (passwordResult.Length == 0 || string.IsNullOrWhiteSpace(passwordResult[0]))
            {
                SetMessage(ErrorMessages.UserNotFoundError);
                SetTitleImage(_celloSadSprite);

                _usernameInput.image.color = Color.red;

                _doTestConnection = true;
                return;
            }

            // Wrong password
            if (!passwordResult[0].Equals(password))
            {
                SetMessage(ErrorMessages.WrongPasswordError);
                SetTitleImage(_celloSadSprite);

                _passwordInput.image.color = Color.red;

                _doTestConnection = true;
                return;
            }

            GetUser((userData) =>
            {
                CurrentUser.SetUser(userData);

                UnitAndAssignmentManager.Instance.DownloadCompletionData(() =>
                {
#if UNITY_EDITOR
                    Debug.Log($"<color=green>Successfully logged int user with ID {userData.UserID}</color>");
#endif
                    SceneManager.LoadScene(1);
                });
            }, username);

        }, select: "password", from: "UserData", where: "username", predicate: username);
    }

    public void TrySubmitLogin() => TrySubmitLogin(_usernameInput.text, _passwordInput.text);

    public void TryRegisterNewUser()
    {
        string username = _usernameInput.text;
        string password = _passwordInput.text;

        if (username.Length == 0)
        {
            SetMessage(ErrorMessages.UsernamePromptEmptyError);
            SetTitleImage(_celloSadSprite);
            return;
        }

        if (password.Length < MinPasswordLength)
        {
            SetMessage(ErrorMessages.PasswordPromptEmptyError);
            SetTitleImage(_celloSadSprite);
            return;
        }

        DB.Instance.Select((checkForDuplicateUsername) =>
        {
            if (checkForDuplicateUsername.Length > 0)
            {
                SetMessage(ErrorMessages.UserAlreadyExistsError);
                SetTitleImage(_celloSadSprite);
                return;
            }

            // Insert new user to db
            DB.Instance.Insert((insert) =>
            {
                AddProgressionData((success) =>
                {
                    if (!success)
                    {
#if UNITY_EDITOR
                        Debug.LogError("Could not add progression data!");
#endif
                        return;
                    }

                    TrySubmitLogin();
                }, username);
            }, username, password, 1, 0, 0);
        }, select: "username", from: "UserData", where: "username", predicate: username);
    }

    private void AddProgressionData(Action<bool> callback, string username)
    {
        DB.Instance.Select((userIDRaw) =>
        {
            if (userIDRaw.Length == 0)
            {
                SetMessage(ErrorMessages.UserIDNotFoundError.Replace("%s", username));

                SetTitleImage(_celloVerySadSprite);
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
        }, select: "userID", from: "UserData", where: "username", predicate: username);
        callback?.Invoke(true);
    }

    public void GetUser(Action<UserData> callback, string username)
    {
        const int coulumCount = 5;

        DB.Instance.Select((result) =>
        {
            if (result.Length - 1 > coulumCount)//-1 because of error code being passed as last element in array
            {
                SetMessage($"Multiple entires in DB found for {username}!");
                SetTitleImage(_celloExtremelySadSprite);
                callback?.Invoke(null);
                return;
            }
            else if (result.Length + 1 < coulumCount)//+1 because of error code being passed as last element in array
            {
                SetMessage("Could not fetch complete data!");
                SetTitleImage(_celloExtremelySadSprite);
                callback?.Invoke(null);
                return;
            }
            else
            {
                callback?.Invoke(new UserData(uint.Parse(result[0]), result[1], result[2], uint.Parse(result[3]), uint.Parse(result[4]), uint.Parse(result[5])));
                return;
            }
        }, select: "*", from: "UserData", where: "username", predicate: username);
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

        if (!Application.isPlaying) return;

        EditorGUILayout.Space();

        if (GUILayout.Button("Login as admin"))
        {
            _loginManager.TrySubmitLogin("admin", "t8734qzp920ßtvhrtbui23op");
        }
    }
}
#endif
