using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private AudioPlayer _failureAudioPlayer;
    [Space]
    [SerializeField] private float CheckConnectionDelay = 2f;
    [SerializeField] private uint MinPasswordLength = 5;

    private float _checkConnectionTimer = 0f;
    private bool _hasCollectionBeenLost = false;
    private bool _doTestConnection = true;

    private void Start() => _checkConnectionTimer = CheckConnectionDelay;

    private void Update()
    {
        _checkConnectionTimer += Time.deltaTime;

        if (_doTestConnection && _checkConnectionTimer < CheckConnectionDelay) return;

        TestConnection();

        _checkConnectionTimer = 0f;
    }

    /// <summary>
    /// Sets the big cello image in the login screen to the given sprite
    /// </summary>
    /// <param name="sprite">The sprite to which the title image should be changed. Leave empty to change it to default cello</param>
    private void SetTitleImage(Sprite sprite = null)
    {
        if (sprite) _titleImage.sprite = sprite;
        else _titleImage.sprite = _celloDefaultSprite;
    }

    /// <summary>
    /// Writes an error message to the bottom of the screen or activates loading animation
    /// </summary>
    /// <param name="msg">The message that should be written to the bottom of the screen. Leave empty to show loading animation instead</param>
    private void SetMessage(string msg = "")
    {
        _messageDisplay.text = msg;

        if (string.IsNullOrWhiteSpace(msg) && _loadingImage) _loadingImage.SetActive(true);
        else if (_loadingImage) _loadingImage.SetActive(false);
    }

    /// <summary>
    /// Makes a test call to the database to check connectivity
    /// </summary>
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

    /// <summary>
    /// Submits login with the username and password from the input fields
    /// </summary>
    public void TrySubmitLogin() => TrySubmitLogin(_usernameInput.text, _passwordInput.text);

    /// <summary>
    /// Submits login with the username from the input field and the given password. Used for submit event
    /// </summary>
    /// <param name="password">The password of the user thta tries to login with the typed username</param>
    public void TrySubmitLogin(string password) => TrySubmitLogin(_usernameInput.text, password);

    /// <summary>
    /// Tries to login the user with the given username and password. If login was successful, retireves user data from the DB
    /// </summary>
    /// <param name="username">The username of the user to login</param>
    /// <param name="password">The password of the user to login</param>
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
                if (_failureAudioPlayer) _failureAudioPlayer.Play();

                _usernameInput.image.color = Color.red;

                _doTestConnection = true;
                return;
            }

            // Wrong password
            if (!VerifyPassword(password, passwordResult[0]))
            {
                SetMessage(ErrorMessages.WrongPasswordError);
                SetTitleImage(_celloSadSprite);
                if (_failureAudioPlayer) _failureAudioPlayer.Play();

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

    /// <summary>
    /// When programming inside the trains of the Deutsche Bahn, I often have no internet.
    /// Thats why a way to login with fake offline user data is neccessary.
    /// </summary>
    public void LoginOffline()
    {
        var userData = new UserData(0, "Offline Demo", "", 0, 0, 0);

        CurrentUser.SetUser(userData);

        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Tries to insert a new user to the database with the given username and password and default statistics
    /// </summary>
    public void TryRegisterNewUser()
    {
        string username = _usernameInput.text;
        string password = _passwordInput.text;

        if (username.Length == 0)
        {
            SetMessage(ErrorMessages.UsernamePromptEmptyError);
            SetTitleImage(_celloSadSprite);
            if (_failureAudioPlayer) _failureAudioPlayer.Play();
            return;
        }

        if (password.Length < MinPasswordLength)
        {
            SetMessage(ErrorMessages.PasswordPromptEmptyError);
            SetTitleImage(_celloSadSprite);
            if (_failureAudioPlayer) _failureAudioPlayer.Play();
            return;
        }

        DB.Instance.Select((checkForDuplicateUsername) =>
        {
            if (checkForDuplicateUsername.Length > 0)
            {
                SetMessage(ErrorMessages.UserAlreadyExistsError);
                SetTitleImage(_celloSadSprite);
                if (_failureAudioPlayer) _failureAudioPlayer.Play();
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
            }, username, EncryptPassword(password), 1, 0, 0);
        }, select: "username", from: "UserData", where: "username", predicate: username);
    }

    /// <summary>
    /// Retrieves the progression data of the user with the given username and saves it in the local arrays
    /// </summary>
    /// <param name="callback">This callback is invoked when the data download is finished</param>
    /// <param name="username">The name of the user of whom the data should be retrieved</param>
    private void AddProgressionData(Action<bool> callback, string username)
    {
        DB.Instance.Select((userIDRaw) =>
        {
            if (userIDRaw.Length == 0)
            {
                SetMessage(ErrorMessages.UserIDNotFoundError.Replace("%s", username));

                SetTitleImage(_celloVerySadSprite);
                if (_failureAudioPlayer) _failureAudioPlayer.Play();
                callback?.Invoke(false);
                return;
            }

            uint userID = uint.Parse(userIDRaw[0]);

            var unitData = CreateProgressionDataArray(userID, UnitAndAssignmentManager.Instance.Units.Length);

            var assignmentData = CreateProgressionDataArray(userID, UnitAndAssignmentManager.Instance.Assignments.Length);

            var achievementData = CreateProgressionDataArray(userID, AchievementManager.Instance.Achievements.Length);

            foreach (var item in unitData)
            {
                DB.Instance.Insert(null, Table.UnitProgress, item[0], item[1], item[2]);
            }

            foreach (var item in assignmentData)
            {
                DB.Instance.Insert(null, Table.AssignmentProgress, item[0], item[1], item[2]);
            }

            foreach (var item in achievementData)
            {
                DB.Instance.Insert(null, Table.AchievementProgress, item[0], item[1], item[2]);
            }
        }, select: "userID", from: "UserData", where: "username", predicate: username);
        callback?.Invoke(true);
    }

    private static uint[][] CreateProgressionDataArray(uint userID, int length)
    {
        var data = new uint[length][];
        for (uint i = 0; i < length; i++)
        {
            data[i] = new uint[]
            {
                i, // link
                0, // isCompleted
                userID
            };
        }

        return data;
    }

    /// <summary>
    /// Retrieves the data from the DB of the user with the given username
    /// </summary>
    /// <param name="callback">This callback is invoked when the download is finished. Carries the informations of the retrieved user.</param>
    /// <param name="username">The name of the user of whom the data should be retrieved</param>
    public void GetUser(Action<UserData> callback, string username)
    {
        const int CoulumCount = 6;

        DB.Instance.Select((result) =>
        {
            if (result.Length - 1 > CoulumCount)//-1 because of error code being passed as last element in array
            {
                SetMessage($"Multiple entires in DB found for {username}!");
                SetTitleImage(_celloExtremelySadSprite);
                if (_failureAudioPlayer) _failureAudioPlayer.Play();
                callback?.Invoke(null);
                return;
            }
            else if (result.Length + 1 < CoulumCount)//+1 because of error code being passed as last element in array
            {
                SetMessage("Could not fetch complete data!");
                SetTitleImage(_celloExtremelySadSprite);
                if (_failureAudioPlayer) _failureAudioPlayer.Play();
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

    private string EncryptPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    private bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
