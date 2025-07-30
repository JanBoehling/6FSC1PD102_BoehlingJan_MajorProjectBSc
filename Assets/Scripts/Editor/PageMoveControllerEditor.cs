using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PageMoveController))]
public class PageMoveControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField($"Current Page: {PageMoveController.Instance.CurrentPage}");

        if (!Application.isPlaying) return;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("<"))
        {
            PageMoveController.Instance.MovePage(-1);
        }

        else if (GUILayout.Button(">"))
        {
            PageMoveController.Instance.MovePage(1);
        }

        EditorGUILayout.EndHorizontal();
    }
    public override bool RequiresConstantRepaint() => true;
}
