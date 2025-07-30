using TMPro;
using UnityEngine;

public class UnitTitleTextUpdater : MonoBehaviour
{
    private TMP_Text _textElement;

    private void Awake() => _textElement = GetComponentInChildren<TMP_Text>();

    private void Start() => UpdateUnitTitleText();

    /// <summary>
    /// Sets the text of the unit button in the middle of the floating buttons to the current units name
    /// </summary>
    public void UpdateUnitTitleText() => _textElement.text = UnitCarousel.GetUnitCarousel().GetCurrentUnitData().Title;
}
