using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QAAssignmentController : AssignmentControllerBase<QAAssignment>
{
    [SerializeField] private QuizCard _quizPrefab;
    [Space]
    [SerializeField] private Button _sendAnswerButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _endMilestoneButton;
    [Space]
    [SerializeField] private QAQuitMessageController _quitMessageContainer;

    private PageMoveController _pages;

    private readonly Dictionary<QuizCard, bool> _loadedQuestions = new();

    private readonly List<AnswerUI> _answerInteractables = new();
    private List<Selectable> _uiInteractables;

    private bool _isDone;

    protected override void Awake()
    {
        base.Awake();
        _pages = GetComponent<PageMoveController>();
    }

    public override void Init(uint assignmentID)
    {
        var questions = AssignmentDataBase.ShuffleArray(_assignmentData.Questions);

        var answerUIPrefab = Resources.Load<AnswerUI>("AssignmentUI/QAAssignment/Answer");

        for (int i = 0; i < questions.Length; i++)
        {
            var item = questions[i];

            var quizUI = Instantiate(_quizPrefab, transform.position + i * ((RectTransform)transform.parent).rect.width * Vector3.right, Quaternion.Euler(0, 0, 0));
            quizUI.transform.SetParent(transform, false);
            quizUI.Init(answerUIPrefab, item, _assignmentID, out var selectables);

            _answerInteractables.AddRange(selectables);
            _loadedQuestions.Add(quizUI, false);
        }

        _uiInteractables = FindObjectsByType<Selectable>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

        _pages.OnMoveAnimationBeginAction = DeactivateInteractables;
        _pages.OnMoveAnimationFinishedAction = ActivateInteractables;
    }

    private void ActivateInteractables()
    {
        ToggleAnswerButtons(true);
        _uiInteractables.ForEach(interactable => interactable.interactable = true);
    }

    private void DeactivateInteractables()
    {
        ToggleAnswerButtons(false);
        _uiInteractables.ForEach(interactable => interactable.interactable = false);
    }

    private void ToggleAnswerButtons(bool isInteractable) => _answerInteractables.ForEach(interactable => interactable.IsInteractable = isInteractable);

    public void CheckAnswer()
    {
        ToggleAnswerButtons(false);

        var answerButtons = _loadedQuestions.ElementAt(_pages.CurrentPage).Key.AnswerButtons;

        var correctAnswers = new List<int>();
        var selectedAnswers = new List<int>();

        for (int i = 0; i < answerButtons.Count; i++)
        {
            var button = answerButtons[i];

            if (button.IsCorrect) correctAnswers.Add(button.Index);
            if (button.IsSelected) selectedAnswers.Add(button.Index);
        }

        bool isSelectionCorrect = correctAnswers.SequenceEqual(selectedAnswers);
        if (isSelectionCorrect)
        {
            _loadedQuestions[_loadedQuestions.ElementAt(_pages.CurrentPage).Key] = true;

            OnCorrectAnswer();
        }
        else
        {
            OnWrongAnswer();
        }

        _sendAnswerButton.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(true);

        // If last page, override continue button action with back to menu
        if (_pages.CurrentPage >= _loadedQuestions.Count - 1)
        {
            _continueButton.onClick = new();

            _endMilestoneButton.GetComponentInChildren<TMP_Text>().text = "Alles gelernt!";

            _isDone = true;
        }
    }

    /// <summary>
    /// Marks the milestone as completed, if the assignment has been completed.
    /// </summary>
    public void FinishAssignment()
    {
        if (!_loadedQuestions.ContainsValue(false))
        {
            _confettiCanon.Play();
            UnitAndAssignmentManager.Instance.SetAssignmentCompletionState(_assignmentID);
            _endMilestoneButton.image.color = Color.green;
            _endMilestoneButton.onClick = new();
            _endMilestoneButton.onClick.AddListener(OnEndMilestone);
        }
        else
        {
            _quitMessageContainer.SelectMessageOnEnable(_isDone ? QAAbortMessage.OnAssignmentFailiure : QAAbortMessage.Abort);
            _quitMessageContainer.gameObject.SetActive(true);
        }
    }

    private void OnCorrectAnswer(bool useDebug = false)
    {
        if (useDebug) Debug.Log("<color=green>Answer correct!");

        _continueButton.image.color = Color.green;

        _confettiCanon.Play();
    }

    private void OnWrongAnswer(bool useDebug = false)
    {
        if (useDebug) Debug.Log("<color=red>Answer wrong!");

        _continueButton.image.color = Color.red;
    }

    public void OnEndMilestone()
    {
        UnitAndAssignmentManager.Instance.UploadCompletionStates();

        if (RuntimeDataHolder.CurrentMilestone.IsCompleted) CurrentUser.RaiseXP(RuntimeDataHolder.CurrentMilestone.XP);

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void ReturnToMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(1);
}
