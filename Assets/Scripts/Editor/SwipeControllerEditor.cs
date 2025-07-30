using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SwipeController))]
public class SwipeControllerEditor : Editor
{
    private SwipeController _controller;

    private void OnEnable() => _controller = (SwipeController)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying) return;
        if (GUILayout.Button("Invoke Drag Event LEFT")) _controller.OnDragEndEventHandler?.Invoke(-1f);
        if (GUILayout.Button("Invoke Drag Event RIGHT")) _controller.OnDragEndEventHandler?.Invoke(1f);
    }
}
