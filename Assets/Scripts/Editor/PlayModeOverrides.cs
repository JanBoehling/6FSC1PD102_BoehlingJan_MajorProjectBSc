using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// This class lets me i.e. enter play mode from whereever i want and still load up the login screen. When entering the game not via the login screen, stuff does not work properly as there is no user data created.
/// </summary>
[InitializeOnLoad]
public static class PlayModeOverrides
{
    #region Menu Paths

    private const string DefaultScenePath = "Assets/Scenes/LoginScreen.unity";

    private const string EnableOverridesMenuItem = "Biolexica/Enable Overrides";
    private const string AutoLoginMenuItem = "Biolexica/Auto Login";
    private const string LoginAsAdminMenuItem = "Biolexica/Login as Admin";

    #endregion

    private static bool EnableOverrides => Menu.GetChecked(EnableOverridesMenuItem);
    private static bool AutoLoginAsAdmin => Menu.GetChecked(AutoLoginMenuItem);

    private static readonly System.Action _loginAsAdmin = () => Object.FindAnyObjectByType<LoginManager>().TrySubmitLogin("admin", "t8734qzp920ßtvhrtbui23op");

    static PlayModeOverrides() => EditorApplication.playModeStateChanged += OnModeSwitch;

    /// <summary>
    /// Enables or disables this script
    /// </summary>
    [MenuItem(EnableOverridesMenuItem, priority = 0)]
    public static void ToggleEnableOverrides() => Menu.SetChecked(EnableOverridesMenuItem, !EnableOverrides);

    /// <summary>
    /// When active, automatically logs in as admin when entering play mode
    /// </summary>
    [MenuItem(AutoLoginMenuItem, priority = 1)]
    public static void ToggleAutoLoginMenu() => Menu.SetChecked(AutoLoginMenuItem, !AutoLoginAsAdmin);

    /// <summary>
    /// Validates the menu item to log the user in as admin. The button may only be active if the login scene is active and the editor is in play mode
    /// </summary>
    /// <returns></returns>
    [MenuItem(LoginAsAdminMenuItem, true)]
    public static bool ValidateLoginMenu() => EditorSceneManager.GetActiveScene().buildIndex == 0 && Application.isPlaying;

    /// <summary>
    /// When in login screen and in play mode, this menu item logs the user in as admin
    /// </summary>
    [MenuItem(LoginAsAdminMenuItem, priority = 2)]
    public static void LoginMenu() => _loginAsAdmin.Invoke();

    /// <summary>
    /// Executes specific behaviour when switching play mode states
    /// </summary>
    /// <param name="state">The current play mode state</param>
    private static void OnModeSwitch(PlayModeStateChange state)
    {
        if (!EnableOverrides) return;

        switch (state)
        {
            case PlayModeStateChange.EnteredEditMode:
                ReturnToPreviousScene();
                break;
            case PlayModeStateChange.ExitingEditMode:
                SwitchToLoginScreen();
                break;
            case PlayModeStateChange.EnteredPlayMode:
                AutoLogin();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returning to previous scene after deactivating play mode
    /// </summary>
    private static void ReturnToPreviousScene()
    {
        if (!EditorPrefs.HasKey("PreviousScenePath")) return;

        EditorSceneManager.OpenScene(EditorPrefs.GetString("PreviousScenePath"));
    }

    /// <summary>
    /// Switching to login screen when entering play mode
    /// </summary>
    private static void SwitchToLoginScreen()
    {
        if (EditorSceneManager.GetActiveScene().path.Equals(DefaultScenePath)) return;

        EditorPrefs.SetString("PreviousScenePath", EditorSceneManager.GetActiveScene().path);

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorApplication.delayCall += () =>
        {
            EditorSceneManager.OpenScene(DefaultScenePath);
            EditorApplication.isPlaying = true;
        };

        EditorApplication.isPlaying = false;
    }

    /// <summary>
    /// Auto login after going into play mode
    /// </summary>
    private static void AutoLogin()
    {
        if (!AutoLoginAsAdmin) return;
        _loginAsAdmin.Invoke();
    }
}
