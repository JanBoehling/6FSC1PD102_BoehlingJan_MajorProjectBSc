using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DeviceOrientationDetector : MonoBehaviour
{
    public (float width, float height) ScreenSizeInWorldSpace { get; private set; }

    [SerializeField] private bool _checkWidth = true;
    [SerializeField] private bool _checkHeight = false;
    [Space]
    [SerializeField] private UnityEvent _onDeviceOrientationChangedEvent;

    private static readonly Func<int> _widthComparator = () => Screen.width;
    private static readonly Func<int> _heightComparator = () => Screen.height;

    private int _width;
    private int _height;

    private void Start()
    {
        ScreenWidthToWorldSpace();
        RecalculateScreenSize();
    }

    private void Update()
    {
        if ((!_checkWidth || _width == _widthComparator.Invoke()) && (!_checkHeight || _height != _heightComparator.Invoke())) return;

        ScreenWidthToWorldSpace();
        RecalculateScreenSize();
    }

    public void RecalculateScreenSize()
    {
        if (_checkWidth) _width = _widthComparator.Invoke();
        if (_checkHeight) _height = _heightComparator.Invoke();

        _onDeviceOrientationChangedEvent?.Invoke();
    }

    public void ScreenWidthToWorldSpace()
    {
        var cam = Camera.main;

        float width = cam.orthographicSize * cam.aspect;
        float height = cam.orthographicSize * 2f;

        ScreenSizeInWorldSpace = (width, height);
    }
}

#if UNITY_EDITOR
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
#endif
