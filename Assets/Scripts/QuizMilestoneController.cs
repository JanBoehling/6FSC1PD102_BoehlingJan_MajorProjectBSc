using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuizMilestoneController : MonoBehaviour
{
    [SerializeField] private QuizAssignmentController _quizPrefab;
    [Space]
    [SerializeField] private Button _sendAnswerButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _endMilestoneButton;

    private PageMoveController _pages;
    private List<QuizAssignmentController> _loadedAssignments = new();

    private void Awake()
    {
        _pages = GetComponent<PageMoveController>();
    }

    private void Start()
    {
        InitQuizzes(RuntimeDataHolder.CurrentMilestone.Assignments);
    }

    private void InitQuizzes(Assignment[] assignments)
    {
        foreach (var item in assignments.Cast<QuizAssignment>())
        {
            var quizUI = Instantiate(item.UIPrefab, transform).GetComponent<QuizAssignmentController>();
            quizUI.Init(item);
            _loadedAssignments.Add(quizUI);
        }
    }

    public void CheckAnswer()
    {
        var answerButtons = _loadedAssignments[_pages.CurrentPage].AnswerButtons;

        var correctAnswers = new List<int>();
        var selectedAnswers = new List<int>();

        for (int i = 0; i < answerButtons.Count; i++)
        {
            var button = answerButtons[i];

            if (button.IsCorrect) correctAnswers.Add(button.Index);
            if (button.IsSelected) selectedAnswers.Add(button.Index);
        }

        bool isSelectionCorrect = correctAnswers.SequenceEqual(selectedAnswers);
        if (isSelectionCorrect) Debug.Log("<color=green>Answer correct!");
        else Debug.Log("<color=red>Answer wrong!");

        _sendAnswerButton.gameObject.SetActive(false);
        if (_loadedAssignments.Count > _pages.CurrentPage) _continueButton.gameObject.SetActive(true);
        else _endMilestoneButton.gameObject.SetActive(true);
    }
}
