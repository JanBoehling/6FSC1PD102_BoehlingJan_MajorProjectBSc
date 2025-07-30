using TMPro;
using UnityEngine;

public class XPDisplay : MonoBehaviour
{
    public static uint XP => CurrentUser.XP;

    public static uint Level => CurrentUser.XP / XpPerLevel;

    private const uint XpPerLevel = 1000;

    [SerializeField] private TMP_Text _xpText;
    [SerializeField] private TMP_Text _levelText;

    private void OnEnable() => UpdateDisplay();

    /// <summary>
    /// Updates the xp and level text elements with the current xp and level
    /// </summary>
    public void UpdateDisplay()
    {
        _xpText.text = $"{XP} XP";
        _levelText.text = Level.ToString();
    }
}
