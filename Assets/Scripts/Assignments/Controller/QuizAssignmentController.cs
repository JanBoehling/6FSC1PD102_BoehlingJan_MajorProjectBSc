using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuizAssignmentController : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private SpriteToTextureConverter _questionImageLoader;
    [SerializeField] private Transform _answerContainer;
    [SerializeField] private QuizAssignment _assignmentData;

    private readonly List<AnswerUI> _answerButtons = new();

    private void Start()
    {
        Init(_assignmentData);
    }

    public void Init(QuizAssignment quiz)
    {
        _questionText.text = quiz.Question;
        _questionImageLoader.LoadImage(quiz.QuestionSprite);

        for (int i = 0; i < quiz.Answers.Length; i++)
        {
            var answer = quiz.Answers[i];

            var answerUIPrefab = Resources.Load<AnswerUI>("AssignmentUI/Answer");
            var answerUI = Instantiate(answerUIPrefab, _answerContainer);
            answerUI.Init(answer.AnswerText, answer.AnswerSprite, answer.IsCorrect, i);
            _answerButtons.Add(answerUI);
        }
    }

    public void CheckAnswer(GameObject container)
    {
        var correctAnswers = new List<int>();
        var selectedAnswers = new List<int>();

        for (int i = 0; i < _answerButtons.Count; i++)
        {
            var button = _answerButtons[i];

            if (button.IsCorrect) correctAnswers.Add(button.Index);
            if (button.IsSelected) selectedAnswers.Add(button.Index);
        }

        bool isSelectionCorrect = correctAnswers.SequenceEqual(selectedAnswers);
        if (isSelectionCorrect) Debug.Log("<color=green>Answer correct!");
        else Debug.Log("<color=red>Answer wrong!");
    }
}
