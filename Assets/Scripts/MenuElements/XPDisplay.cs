using TMPro;
using UnityEngine;

public class XPDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _xpText;
    [SerializeField] private TMP_Text _levelText;

    public static uint XP => CurrentUser.XP;

    public static uint Level => CurrentUser.XP / XpPerLevel;

    private const uint XpPerLevel = 1000;

    private void OnEnable()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        _xpText.text = $"{XP} XP";
        _levelText.text = Level.ToString();
    }
}
