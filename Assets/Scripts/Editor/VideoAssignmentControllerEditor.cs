using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VideoAssignmentController))]
public class VideoAssignmentControllerEditor : Editor
{
    private VideoAssignmentController _assignmentController;

    private void OnEnable() => _assignmentController = (VideoAssignmentController)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying) return;

        // Displays the current watchtime and the duration of the video
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox($"{_assignmentController.CurrentWatchtime:F2} / {_assignmentController.VideoDuration:F2}", MessageType.None, true);

        EditorGUILayout.Separator();

        if (GUILayout.Button("Force Play Video")) _assignmentController.Init(_assignmentController.DebugVideoClip);
    }

    public override bool RequiresConstantRepaint() => true;
}
