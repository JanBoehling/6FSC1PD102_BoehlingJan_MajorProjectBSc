using System;
using UnityEngine;

[CreateAssetMenu]
public class QAAssignment : AssignmentDataBase
{
    [SerializeField] private Question[] _questions;

    public Question[] Questions => _questions;

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
