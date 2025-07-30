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
        _basePosition = _transform.anchoredPosition;
    }

    /// <summary>
    /// Shuffles all questions, loads the answers UI and gets all selectable UI elements in the scene
    /// </summary>
    /// <param name="assignmentID">The ID of the assignment</param>
    public override void Init(uint assignmentID)
    {
        var questions = AssignmentDataBase.ShuffleArray(AssignmentData.Questions);

        _answerUIPrefab = Resources.Load<AnswerUI>("AssignmentUI/QAAssignment/Answer");

        AddQuizCards(questions);

        _uiInteractables = FindObjectsByType<Selectable>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

        _pages.OnMoveAnimationBeginAction = DeactivateInteractables;
        _pages.OnMoveAnimationFinishedAction = ActivateInteractables;
    }

    /// <summary>
    /// Instantiates a new range of quiz cards
    /// </summary>
    /// <param name="questions">The range of new quiz cards that should be added</param>
    public void AddQuizCards(Question[] questions)
    {
        for (int i = 0; i < questions.Length; i++)
        {
            var item = questions[i];

            var quizUI = Instantiate(_quizPrefab, transform);
            SetQuizCardPosition(quizUI);
            quizUI.Init(_answerUIPrefab, item, AssignmentID, out var selectables);

            _answerInteractables.AddRange(selectables);
            _loadedQuestions.Add(quizUI, false);
        }
    }

    /// <summary>
    /// Sets the position and offset of all quiz cards as well as the assignment container
    /// </summary>
    public void SetCardPositions()
    {
        // Sets card width to fill viewport width
        for (int i = 0; i < _loadedQuestions.Keys.Count; i++)
        {
            SetQuizCardPosition(_loadedQuestions.ElementAt(i).Key);
        }
        
        // Sets container offset
        _transform.anchoredPosition = _canvasTransform.rect.width * _pages.CurrentPage * Vector2.left + _basePosition;
    }

    /// <summary>
    /// Sets the position of the given quiz card based on the width of the parent canvas
    /// </summary>
    /// <param name="card">the card that should be repositioned</param>
    private void SetQuizCardPosition(QuizCard card) => card.GetComponent<LayoutElement>().minWidth = _canvasTransform.rect.width;

    /// <summary>
    /// Activates all UI buttons
    /// </summary>
    private void ActivateInteractables()
    {
        ToggleAnswerButtons(true);
        _uiInteractables.ForEach(interactable => interactable.interactable = true);
    }

    /// <summary>
    /// Deactivates all UI buttons
    /// </summary>
    private void DeactivateInteractables()
    {
        ToggleAnswerButtons(false);
        _uiInteractables.ForEach(interactable => interactable.interactable = false);
    }

    /// <summary>
    /// Toggles the interactable state of all UI buttons, like 'Check' or 'End Milestone'
    /// </summary>
    /// <param name="isInteractable">Whether or not the interactables should be interactable</param>
    private void ToggleAnswerButtons(bool isInteractable) => _answerInteractables.ForEach(interactable => interactable.IsInteractable = isInteractable);

    /// <summary>
    /// Checks if the selected answers are correct
    /// </summary>
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
        if (_pages.CurrentPage < _loadedQuestions.Count - 1) return;
        OnLastQuestion();
    }

    /// <summary>
    /// When the last question has been answered, deactivates the continue button and overrides the end milestone button to activate the end message
    /// </summary>
    private void OnLastQuestion()
    {
        _continueButton.onClick.RemoveAllListeners();
        _continueButton.transform.parent.gameObject.SetActive(false);

        _endMilestoneButton.GetComponentInChildren<TMP_Text>().text = _quitButtonTextOnEnd;
        _endMilestoneButton.image.color = Color.green;
        _endMilestoneButton.onClick.RemoveAllListeners();
        _endMilestoneButton.onClick.AddListener(() =>
        {
            _quitMessageContainer.SelectMessageOnEnable(QAAbortMessage.None);
            _quitMessageContainer.DisplayText($"+{RuntimeDataHolder.CurrentMilestone.XP} XP!");
            _quitMessageContainer.CloseMessageButton.gameObject.SetActive(false);
            _quitMessageContainer.ConfirmAbortButton.onClick.RemoveAllListeners();
            _quitMessageContainer.ConfirmAbortButton.onClick.AddListener(OnEndMilestone);
            _quitMessageContainer.gameObject.SetActive(true);
        });

        _isDone = true;
    }

    /// <summary>
    /// Marks the milestone as completed, if the assignment has been completed.
    /// </summary>
    public void FinishAssignment()
    {
        if (_isDone) return;

        _quitMessageContainer.SelectMessageOnEnable(QAAbortMessage.Abort);
        _closeQuitMessageButton.SetActive(true);
        _quitMessageContainer.gameObject.SetActive(true);
    }

    /// <summary>
    /// When the answer was correct, the button turns green and a confetti effect plays
    /// </summary>
    private void OnCorrectAnswer()
    {
        _continueButton.image.color = Color.green;

        ConfettiCanon.Play();
    }

    /// <summary>
    /// When the answer was false, the button turns red and the quiz card is duplicated
    /// </summary>
    private void OnWrongAnswer()
    {
        _continueButton.image.color = Color.red;

        AddQuizCards(new[]{ _loadedQuestions.ElementAt(_pages.CurrentPage).Key.Question });
    }

    /// <summary>
    /// When exiting the finished milestone, updates the completion state arrays, raises the users XP and returns to the main menu
    /// </summary>
    public void OnEndMilestone()
    {
        UnitAndAssignmentManager.Instance.SetAssignmentCompletionState(AssignmentID);

        UnitAndAssignmentManager.Instance.UploadCompletionStates();

        if (RuntimeDataHolder.CurrentMilestone.IsCompleted) CurrentUser.RaiseXP(RuntimeDataHolder.CurrentMilestone.XP);

        ReturnToMenu();
    }

    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    public void ReturnToMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(1);
}
