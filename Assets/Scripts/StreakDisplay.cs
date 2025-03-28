using UnityEngine;

public class StreakDisplay : MonoBehaviour
{
    private void OnEnable()
    {
        StreakManager.Instance.UpdateDisplay();
    }
}
