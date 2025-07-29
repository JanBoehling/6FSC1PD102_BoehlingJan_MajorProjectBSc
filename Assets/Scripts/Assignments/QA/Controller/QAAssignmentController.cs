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
    [SerializeField] private string _quitButtonTextOnEnd = "Das war's!";
    [SerializeField] private QAQuitMessageController _quitMessageContainer;
    [SerializeField] private GameObject _closeQuitMessageButton;

    private RectTransform _canvasTransform;
    private RectTransform _transform;

    private AnswerUI _answerUIPrefab;

    private PageMoveController _pages;
    private DeviceOrientationDetector _orientation;

    private readonly Dictionary<QuizCard, bool> _loadedQuestions = new();

    private readonly List<AnswerUI> _answerInteractables = new();
    private List<Selectable> _uiInteractables;

    private bool _isDone;

    private Vector2 _basePosition;

    protected override void Awake()
    {
        base.Awake();
        _transform = GetComponent<RectTransform>();
        _canvasTransform = FindFirstObjectByType<Canvas>().GetComponent<RectTransform>();
        _pages = GetComponent<PageMoveController>();
        _orientation = GetComponent<DeviceOrientationDetector>();
        _basePosition = _transform.anchoredPosition;
    }

    public override void Init(uint assignmentID)
    {
        var questions = AssignmentDataBase.ShuffleArray(_assignmentData.Questions);

        _answerUIPrefab = Resources.Load<AnswerUI>("AssignmentUI/QAAssignment/Answer");

        AddQuizCards(questions);

        _uiInteractables = FindObjectsByType<Selectable>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

        _pages.OnMoveAnimationBeginAction = DeactivateInteractables;
        _pages.OnMoveAnimationFinishedAction = ActivateInteractables;
    }

    public void AddQuizCards(Question[] questions)
    {
        for (int i = 0; i < questions.Length; i++)
        {
            var item = questions[i];

            var quizUI = Instantiate(_quizPrefab, transform);
            quizUI.GetComponent<RectTransform>().sizeDelta = new(_canvasTransform.sizeDelta.x, _canvasTransform.sizeDelta.y);

            quizUI.Init(_answerUIPrefab, item, _assignmentID, out var selectables);

            _answerInteractables.AddRange(selectables);
            _loadedQuestions.Add(quizUI, false);
        }
    }

    public void SetCardPositions()
    {
        // Sets card width to fill viewport width
        for (int i = 0; i < _loadedQuestions.Keys.Count; i++)
        {
            _loadedQuestions.ElementAt(i).Key.GetComponent<LayoutElement>().minWidth = _canvasTransform.rect.width;
        }
        
        // Sets container offset
        _transform.anchoredPosition = _canvasTransform.rect.width * _pages.CurrentPage * Vector2.left + _basePosition;
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
        if (_pages.CurrentPage < 0) return;

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
            _continueButton.transform.parent.gameObject.SetActive(false);

            _endMilestoneButton.GetComponentInChildren<TMP_Text>().text = _quitButtonTextOnEnd;
            _endMilestoneButton.image.color = Color.green;
            _endMilestoneButton.onClick = new();
            _endMilestoneButton.onClick.AddListener(OnEndMilestone);

            _isDone = true;
        }
    }

    /// <summary>
    /// Marks the milestone as completed, if the assignment has been completed.
    /// </summary>
    public void FinishAssignment()
    {
        if (!_isDone)
        {
            _quitMessageContainer.SelectMessageOnEnable(QAAbortMessage.Abort);
            _closeQuitMessageButton.SetActive(true);
            _quitMessageContainer.gameObject.SetActive(true);
            return;
        }

        /* Forcing the user to have every question correct while not giving them the opportunity to correct themselves, turned out to not be a good idea.
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
            _closeQuitMessageButton.SetActive(!_isDone);
            _quitMessageContainer.gameObject.SetActive(true);
        }
        */
    }

    private void OnCorrectAnswer(bool useDebug = false)
    {
#if UNITY_EDITOR
        if (useDebug) Debug.Log("<color=green>Answer correct!");
#endif

        _continueButton.image.color = Color.green;

        _confettiCanon.Play();
    }

    private void OnWrongAnswer(bool useDebug = false)
    {
#if UNITY_EDITOR
        if (useDebug) Debug.Log("<color=red>Answer wrong!");
#endif

        _continueButton.image.color = Color.red;

        AddQuizCards(new[]{ _loadedQuestions.ElementAt(_pages.CurrentPage).Key.Question });
    }

    public void OnEndMilestone()
    {
        UnitAndAssignmentManager.Instance.SetAssignmentCompletionState(_assignmentID);

        UnitAndAssignmentManager.Instance.UploadCompletionStates();

        if (RuntimeDataHolder.CurrentMilestone.IsCompleted) CurrentUser.RaiseXP(RuntimeDataHolder.CurrentMilestone.XP);

        ReturnToMenu();
    }

    public void ReturnToMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(1);
}
