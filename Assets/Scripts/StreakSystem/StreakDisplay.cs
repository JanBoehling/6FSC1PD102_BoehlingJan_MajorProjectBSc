using UnityEngine;

public class StreakDisplay : MonoBehaviour
{
    private StreakManager _streakManager;

    private void Awake()
    {
        var streakManager = FindObjectsByType<StreakManager>(FindObjectsSortMode.None);

        if (streakManager.Length < 1) Debug.LogError("Error: Could not find StreakManager in scene!");
        else if (streakManager.Length > 1) Debug.LogError("Error: Multiple StreakManagers in scene!");
        else _streakManager = streakManager[0];
    }

    private void OnEnable()
    {
        _streakManager.UpdateDisplay();
    }
}
