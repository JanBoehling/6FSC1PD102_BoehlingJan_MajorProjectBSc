using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugManager))]
public class DebugManagerEditor : Editor
{
    private DebugManager _debugManager;

    private void OnEnable() => _debugManager = (DebugManager)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh User Data")) CurrentUser.SetUser(_debugManager.DebugUserData);
    }
}
