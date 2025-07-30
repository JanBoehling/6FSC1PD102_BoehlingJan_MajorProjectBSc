using UnityEngine;

/// <summary>
/// This class is used in combination with the editor script
/// </summary>
public class DebugManager : MonoBehaviour
{
    [SerializeField] private bool _useDebugUserData;
    [field: SerializeField] public UserData DebugUserData { get; private set; } = new(0, "Test", "pwd", 0, 69, 0);

    private void Awake()
    {
        if (_useDebugUserData) CurrentUser.SetUser(DebugUserData);
    }
}
