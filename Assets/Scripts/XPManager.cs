using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPManager : MonoSingleton<XPManager>
{
    [SerializeField] private Slider _xpSlider;
    [SerializeField] private TMP_Text _levelText;

    public static int XP { get; private set; }

    public static int Level { get; private set; }

    private const int XpPerLevel = 1000;

    private void Start()
    {
        UpdateXPDisplay();
    }

    public void RaiseXP(int value)
    {
        XP += value;

        if (XP >= XpPerLevel)
        {
            XP -= XpPerLevel;
            Level++;
        }

        UpdateXPDisplay();
    }

    private void UpdateXPDisplay()
    {
        _xpSlider.value = XP;
        _levelText.text = Level.ToString();
    }
}
