using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizCard : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private ImageLoader _questionImageLoader;
    [SerializeField] private Transform _answerContainer;

    private readonly List<AnswerUI> _answerButtons = new();
    public List<AnswerUI> AnswerButtons => _answerButtons;

    public uint AssignmentID { get; private set; }

    public void Init(AnswerUI answerUIPrefab, Question question, uint assignmentID)
    {
        _questionText.text = question.QuestionText;
        _questionImageLoader.LoadImage(question.QuestionSprite);

        var answers = AssignmentDataBase.ShuffleArray(question.Answers);

        for (int i = 0; i < answers.Length; i++)
        {
            var answer = answers[i];

            var answerUI = Instantiate(answerUIPrefab, _answerContainer);
            answerUI.Init(answer.AnswerText, answer.AnswerSprite, answer.IsCorrect, i);
            _answerButtons.Add(answerUI);
        }

        AssignmentID = assignmentID;
    }

    [System.Obsolete("Use the new QAAssignment")]
    public void Init(uint assignmentID)
    {
        var quiz = UnitAndAssignmentManager.Instance.GetAssignmentByID(assignmentID) as QuizAssignment;

        _questionText.text = quiz.Question;
        _questionImageLoader.LoadImage(quiz.QuestionSprite);

        var answers = QAAssignment.ShuffleArray(quiz.Answers);

        for (int i = 0; i < quiz.Answers.Length; i++)
        {
            var answer = answers[i];

            var answerUIPrefab = Resources.Load<AnswerUI>("AssignmentUI/Answer");
            var answerUI = Instantiate(answerUIPrefab, _answerContainer);
            answerUI.Init(answer.AnswerText, answer.AnswerSprite, answer.IsCorrect, i);
            _answerButtons.Add(answerUI);
        }

        AssignmentID = assignmentID;
    }
}
