using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class SafeAreaContentFitter : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Camera _cam;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _cam = Camera.main;
    }

    private void Start()
    {
        FitToSafeArea();
    }

    /// <summary>
    /// Fetches the devices safe area and calculates the anchor offset by using the camera's width and height.<br/>
    /// GameObject needs to be on fullscreen-streched anchor preset!
    /// </summary>
    public void FitToSafeArea()
    {
        if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
        if (!_cam) _cam = Camera.main;

        var safeArea = Screen.safeArea;
        
        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= _cam.pixelWidth;
        anchorMin.y /= _cam.pixelHeight;

        anchorMax.x /= _cam.pixelWidth;
        anchorMax.y /= _cam.pixelHeight;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SafeAreaContentFitter))]
public class SafeAreaContentFitterEditor : Editor
{
    private SafeAreaContentFitter _contentFitter;
    private bool _foldout;

    private void OnEnable()
    {
        _contentFitter = (SafeAreaContentFitter)target;
    }

    public override void OnInspectorGUI()
    {
        _foldout = EditorGUILayout.Foldout(_foldout, "Safe Area");
        if (_foldout) EditorGUILayout.RectField(Screen.safeArea);

        if (!Application.isPlaying) return;

        if (GUILayout.Button("Fit content to safe area")) _contentFitter.FitToSafeArea();
    }
}
#endif
