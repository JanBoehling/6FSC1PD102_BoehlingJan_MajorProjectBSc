using UnityEngine;

[CreateAssetMenu]
public class QuizAssignment : Assignment
{
    [SerializeField] private string _question;
    [SerializeField, Tooltip("Optional")] private Sprite _questionSprite;

    [SerializeField] private Answer[] _answers;

    public string Question => _question;
    public Sprite QuestionSprite => _questionSprite;
    public Answer[] Answers => _answers;

    public bool CheckSelection(params int[] selectedIndices)
    {
        foreach (var index in selectedIndices)
        {
            if (!_answers[index].IsCorrect) return false;
        }

        return true;
    }
}
