using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoginManager))]
public class LoginManagerEditor : Editor
{
    private LoginManager _loginManager;

    private void OnEnable() => _loginManager = (LoginManager)target;

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
