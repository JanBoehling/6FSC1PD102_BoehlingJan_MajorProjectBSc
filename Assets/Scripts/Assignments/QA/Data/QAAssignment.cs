using UnityEngine;

[CreateAssetMenu]
public class QAAssignment : AssignmentDataBase
{
    [SerializeField] private Question[] _questions;

    public Question[] Questions => _questions;

    /// <summary>
    /// Checks whether the answers of the question at the given index were correctly selected
    /// </summary>
    /// <param name="questionIndex">The index of the current question</param>
    /// <param name="selectedIndices">The indices of all the selected answer buttons</param>
    /// <returns>Whether or not the question was answered correctly</returns>
    public bool CheckSelection(int questionIndex, params int[] selectedIndices)
    {
        var answers = _questions[questionIndex].Answers;

        foreach (var index in selectedIndices)
        {
            if (!answers[index].IsCorrect) return false;
        }

        return true;
    }
}
