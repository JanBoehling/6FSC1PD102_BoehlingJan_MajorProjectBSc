using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugManager : MonoBehaviour
{
    [SerializeField] private bool _useDebugUserData;
    [field: SerializeField] public UserData DebugUserData { get; private set; } = new(0, "Test", "pwd", 0, 69);

    private void Awake()
    {
        if (_useDebugUserData) CurrentUser.SetUser(DebugUserData);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DebugManager))]
public class DebugManagerEditor : Editor
{
    private DebugManager _debugManager;

    private void OnEnable()
    {
        _debugManager = (DebugManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh User Data")) CurrentUser.SetUser(_debugManager.DebugUserData);
    }
}
#endif
