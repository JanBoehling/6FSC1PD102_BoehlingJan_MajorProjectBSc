using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitListItem : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _titleText;

    public void InitUnitListItem(Sprite icon, string title)
    {
        _iconImage.sprite = icon;
        _titleText.text = title;
    }
}
