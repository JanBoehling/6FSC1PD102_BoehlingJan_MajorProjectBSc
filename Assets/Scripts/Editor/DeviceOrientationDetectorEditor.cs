using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeviceOrientationDetector))]
public class DeviceOrientationDetectorEditor : Editor
{
    private DeviceOrientationDetector _detector;

    private void OnEnable() => _detector = (DeviceOrientationDetector)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();

        if (!GUILayout.Button("Recalculate Screen Size")) return;
        _detector.RecalculateScreenSize();
    }
}
