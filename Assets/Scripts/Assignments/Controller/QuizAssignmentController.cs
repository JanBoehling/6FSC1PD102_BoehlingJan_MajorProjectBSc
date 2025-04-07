using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuizAssignmentController : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private SpriteToTextureConverter _questionImageLoader;
    [SerializeField] private Transform _answerContainer;

    private readonly List<AnswerUI> _answerButtons = new();
    public List<AnswerUI> AnswerButtons => _answerButtons;

    public QuizAssignment AssignmentData { get; private set; }

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

        AssignmentData = quiz;
    }
}
