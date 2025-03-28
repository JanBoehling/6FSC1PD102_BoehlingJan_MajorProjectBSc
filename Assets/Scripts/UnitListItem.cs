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

    public void Init(Milestone milestone)
    {
        _iconImage.sprite = milestone.Icon;
        _titleText.text += milestone.Title;

        var button = GetComponent<Button>();
        button.onClick.AddListener(() => milestone.Assignments[0].LoadAssignmentUI());
        //button.onClick.AddListener(UnitOverviewController.Instance.Hide);
    }
}
