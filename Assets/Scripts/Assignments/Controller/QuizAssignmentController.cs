using TMPro;
using UnityEngine;

public class QuizAssignmentController : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private QuestionImageLoader _questionImageLoader;
    [SerializeField] private Transform _answerContainer;

    private void OnEnable()
    {
        Init();
    }

    public void Init(QuizAssignment quiz)
    {
        _questionText.text = quiz.Question;
        _questionImageLoader.LoadImage(quiz.QuestionSprite);

        foreach (var item in quiz.Answers)
        {
            Debug.Log(item);
            var answerUIPrefab = Resources.Load<AnswerUI>("AssignmentUI/Answer");
            var answer = Instantiate(answerUIPrefab, _answerContainer);
            answer.Init(item.AnswerText, item.AnswerSprite);
        }
    }
}
