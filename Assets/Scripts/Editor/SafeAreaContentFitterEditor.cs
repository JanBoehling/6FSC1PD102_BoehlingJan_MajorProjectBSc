using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SafeAreaContentFitter))]
public class SafeAreaContentFitterEditor : Editor
{
    private SafeAreaContentFitter _contentFitter;
    private bool _foldout;

    private void OnEnable() => _contentFitter = (SafeAreaContentFitter)target;

    public override void OnInspectorGUI()
    {
        _foldout = EditorGUILayout.Foldout(_foldout, "Safe Area");
        if (_foldout) EditorGUILayout.RectField(Screen.safeArea);

        if (!Application.isPlaying) return;

        if (GUILayout.Button("Fit content to safe area")) _contentFitter.FitToSafeArea();
    }
}
