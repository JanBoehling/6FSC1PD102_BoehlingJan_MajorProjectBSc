using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizMilestoneController : MonoBehaviour
{
    [SerializeField] private bool _debugOverrideMilestone;
    [SerializeField] private Assignment[] _debugAssignments;
    [Space]
    [SerializeField] private QuizAssignmentController _quizPrefab;
    [Space]
    [SerializeField] private Button _sendAnswerButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _endMilestoneButton;

    private PageMoveController _pages;
    private List<QuizAssignmentController> _loadedAssignments = new();

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

    private void InitQuizzes(Assignment[] assignments)
    {
        /*foreach (var item in assignments.Cast<QuizAssignment>())
        {
            var quizUI = Instantiate(item.UIPrefab, transform.position + _pages.CurrentPage * ((RectTransform)transform).rect.width * Vector3.right, Quaternion.identity, transform).GetComponent<QuizAssignmentController>();
            quizUI.Init(item);
            _loadedAssignments.Add(quizUI);
        }*/

        for (int i = 0; i < assignments.Length; i++)
        {
            var item = assignments[i] as QuizAssignment;

            var quizUI = Instantiate(item.UIPrefab, transform.position + i * Screen.width * Vector3.right, Quaternion.Euler(0, 0, 0)).GetComponent<QuizAssignmentController>();
            quizUI.transform.SetParent(transform, false);
            quizUI.Init(item);
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
            currentAssignment.AssignmentData.IsCompleted = true;
            
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
            _continueButton.GetComponentInChildren<TMP_Text>().text = "Und wieder zurück!";
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
