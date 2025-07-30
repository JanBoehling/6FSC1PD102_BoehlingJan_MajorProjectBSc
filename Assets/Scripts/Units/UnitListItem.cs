using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitListItem : MonoBehaviour
{
    [SerializeField] private Image _buttonBGImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _titleText;
    [Space]
    [SerializeField] private Color _baseColor = Color.red;
    [SerializeField] private Color _completedColor = Color.green;
    [Space]
    [SerializeField] private Sprite _videoIcon;
    [SerializeField] private Sprite _assignmentQAIcon;

    /// <summary>
    /// Retrieves the assignment data of the given milestone, sets the title text and assignment icon, deposits the button logic and colors the button based on completion state
    /// </summary>
    /// <param name="milestone">The milestone that the button is connected with</param>
    public void Init(Milestone milestone)
    {
        var assignmentData = UnitAndAssignmentManager.Instance.GetAssignmentByID(milestone.Assignments[0]);

        _titleText.text = milestone.Title;
        _iconImage.sprite = assignmentData switch // I love pattern matching. Best feature in C#
        {
            VideoAssignment _ => _videoIcon,
            QAAssignment _ => _assignmentQAIcon,
            _ => null
        };

        // Fallback design settings
        if (_iconImage.sprite == null)
        {
            _iconImage.gameObject.SetActive(false);
            _titleText.rectTransform.offsetMin = new(-_titleText.rectTransform.offsetMax.x, _titleText.rectTransform.offsetMin.y);
            _titleText.horizontalAlignment = HorizontalAlignmentOptions.Center;
        }

        var button = GetComponent<Button>();
        button.onClick.AddListener(() => RuntimeDataHolder.CurrentMilestone = milestone);

        button.onClick.AddListener(() => assignmentData.LoadAssignmentUI());

        if (milestone.IsCompleted) _buttonBGImage.color = _completedColor;
        else _buttonBGImage.color = _baseColor;
    }
}
