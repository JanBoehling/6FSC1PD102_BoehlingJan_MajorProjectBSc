using TMPro;
using UnityEngine;

public class XPManager : MonoSingleton<XPManager>
{
    [SerializeField] private TMP_Text _xpText;
    [SerializeField] private TMP_Text _levelText;

    public static int XP { get; private set; }

    public static int Level { get; private set; }

    private const int XpPerLevel = 1000;

    private void Start()
    {
        UpdateDisplay();
    }

    public void RaiseXP(int value)
    {
        XP += value;

        if (XP >= XpPerLevel)
        {
            XP -= XpPerLevel;
            Level++;
        }

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        _xpText.text = $"{XP} XP";
        _levelText.text = Level.ToString();
    }
}
