using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// This class lets me enter play mode from whereever i want and still load up the login screen. When entering the game not via the login screen, stuff does not work properly as there is no user data created.
/// </summary>
[InitializeOnLoad]
public class EnterPlaymodeController
{
    private const string DefaultScenePath = "Assets/Scenes/LoginScreen.unity";

    static EnterPlaymodeController() => EditorApplication.playModeStateChanged += LoadDefaultScene;

    private static void LoadDefaultScene(PlayModeStateChange state)
    {
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

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (!PlayerPrefs.HasKey("PreviousScenePath")) return;

            EditorSceneManager.OpenScene(PlayerPrefs.GetString("PreviousScenePath"));
        }
    }
}
