using TMPro;
using UnityEngine;

public class XPManager : MonoSingleton<XPManager>
{
    [SerializeField] private TMP_Text _xpText;
    [SerializeField] private TMP_Text _levelText;

    public static uint XP => CurrentUser.XP;

    public static uint Level => CurrentUser.XP / XpPerLevel;

    private const uint XpPerLevel = 1000;

    private void Start()
    {
        UpdateDisplay();
    }

    public void RaiseXP(int value)
    {
        CurrentUser.RaiseXP(value);

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        _xpText.text = $"{XP} XP";
        _levelText.text = Level.ToString();
    }
}
