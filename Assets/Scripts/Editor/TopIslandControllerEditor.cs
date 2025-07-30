using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TopIslandController))]
public class TopIslandControllerEditor : Editor
{
    private TopIslandController _controller;

    private string _debugTitle;

    private void OnEnable() => _controller = (TopIslandController)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        if (GUILayout.Button("Toggle Title")) _controller.ToggleTitle();
        _debugTitle = EditorGUILayout.TextField(_debugTitle);
        if (GUILayout.Button("Display Title")) _controller.DisplayTitle(_debugTitle);
    }
}
