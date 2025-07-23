using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// This class lets me enter play mode from whereever i want and still load up the login screen. When entering the game not via the login screen, stuff does not work properly as there is no user data created.
/// </summary>
[InitializeOnLoad]
public class EnterPlaymodeController
{
    #region Settings

    private const string DefaultScenePath = "Assets/Scenes/LoginScreen.unity";

    private const bool AutoLoginAsAdmin = true;

    #endregion

    static EnterPlaymodeController() => EditorApplication.playModeStateChanged += LoadDefaultScene;

    private static void LoadDefaultScene(PlayModeStateChange state)
    {
        // Switching to login screen when entering play mode
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (EditorSceneManager.GetActiveScene().path.Equals(DefaultScenePath)) return;

            PlayerPrefs.SetString("PreviousScenePath", EditorSceneManager.GetActiveScene().path);
            PlayerPrefs.Save();

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            EditorApplication.delayCall += () =>
            {
                EditorSceneManager.OpenScene(DefaultScenePath);
                EditorApplication.isPlaying = true;
            };

            EditorApplication.isPlaying = false;
        }

        // Returning to previous scene after deactivating play mode
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (!PlayerPrefs.HasKey("PreviousScenePath")) return;

            EditorSceneManager.OpenScene(PlayerPrefs.GetString("PreviousScenePath"));
        }

        // Auto login after going into play mode
        else if (state == PlayModeStateChange.EnteredPlayMode)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (!AutoLoginAsAdmin) return;
#pragma warning restore CS0162 // Unreachable code detected
            Object.FindAnyObjectByType<LoginManager>().TrySubmitLogin("admin", "t8734qzp920ßtvhrtbui23op");
        }
    }
}
