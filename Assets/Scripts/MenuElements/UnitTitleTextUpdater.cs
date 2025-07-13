using TMPro;
using UnityEngine;

public class UnitTitleTextUpdater : MonoBehaviour
{
    private TMP_Text _textElement;

    private void Awake()
    {
        _textElement = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        UpdateUnitTitleText();
    }

    public void UpdateUnitTitleText()
    {
        var currentUnit = UnitCarousel.GetUnitCarousel().GetCurrentUnitData();
        _textElement.text = currentUnit.Title;
    }
}
