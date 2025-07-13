using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitListItem : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _titleText;

    [SerializeField] private Color _baseColor = Color.red;
    [SerializeField] private Color _completedColor = Color.green;

    public void InitUnitListItem(Sprite icon, string title)
    {
        _titleText.text = title;
    }

    public void Init(Milestone milestone)
    {
        _titleText.text = milestone.Title;

        var button = GetComponent<Button>();
        button.onClick.AddListener(() => RuntimeDataHolder.CurrentMilestone = milestone);
        //button.onClick.AddListener(() => CompletionTracker.Instance.CurrentMilestone = milestone);

        button.onClick.AddListener(() => UnitAndAssignmentManager.Instance.GetAssignmentByID(milestone.Assignments[0]).LoadAssignmentUI());

        if (milestone.IsCompleted) _iconImage.color = _completedColor;
        else _iconImage.color = _baseColor;
    }
}
