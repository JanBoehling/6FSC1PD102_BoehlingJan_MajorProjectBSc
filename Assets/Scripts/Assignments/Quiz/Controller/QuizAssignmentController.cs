using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Obsolete("Use QA Assignment Controller")]
public class QuizAssignmentController : MonoBehaviour
{
    [SerializeField] private bool _debugOverrideMilestone;
    [SerializeField] private uint[] _debugAssignments;
    [Space]
    [SerializeField] private QuizCard _quizPrefab;
    [Space]
    [SerializeField] private Button _sendAnswerButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _endMilestoneButton;

    private PageMoveController _pages;
    private List<QuizCard> _loadedAssignments = new();

    private ParticleSystem _confettiCanon;

    private void Awake()
    {
        _pages = GetComponent<PageMoveController>();
        _confettiCanon = FindFirstObjectByType<ParticleSystem>();
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (_debugOverrideMilestone && _debugAssignments is not null && _debugAssignments.Length > 0)
        {
            InitQuizzes(_debugAssignments);
        }
        else
#endif
            InitQuizzes(RuntimeDataHolder.CurrentMilestone.Assignments);
    }

    private void InitQuizzes(uint[] assignments)
    {
        for (int i = 0; i < assignments.Length; i++)
        {
            var item = UnitAndAssignmentManager.Instance.GetAssignmentByID(assignments[i]);

            var quizUI = Instantiate(item.UIPrefab, transform.position + i * Screen.width * Vector3.right, Quaternion.Euler(0, 0, 0)).GetComponent<QuizCard>();
            quizUI.transform.SetParent(transform, false);
            quizUI.Init(assignments[i]);
            _loadedAssignments.Add(quizUI);
        }
    }

    public void CheckAnswer()
    {
        var currentAssignment = _loadedAssignments[_pages.CurrentPage];
        var answerButtons = currentAssignment.AnswerButtons;

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
            UnitAndAssignmentManager.Instance.SetAssignmentCompletionState(currentAssignment.AssignmentID);
            
            OnCorrectAnswer();
        }
        else
        {
            OnWrongAnswer();
        }

        _sendAnswerButton.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(true);

        // If last page, override continue button action with back to menu
        if (_pages.CurrentPage >= _loadedAssignments.Count - 1)
        {
            _continueButton.GetComponentInChildren<TMP_Text>().text = "Und wieder zur�ck!";
            _continueButton.onClick = new();
            _continueButton.onClick.AddListener(OnEndMilestone);
            _continueButton.gameObject.SetActive(true);
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
}
