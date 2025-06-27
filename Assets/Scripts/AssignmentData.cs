using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AssignmentData : ScriptableObject
{
    [SerializeField] private int _assignmentSceneIndex;
    [SerializeField] private GameObject _uiPrefab;
    public GameObject UIPrefab => _uiPrefab;

    /// <summary>
    /// Randomly shuffles the items of the given array.
    /// </summary>
    /// <typeparam name="T">Array type</typeparam>
    /// <param name="original">The source array that shoudl be shuffled</param>
    /// <returns>A shuffled copy of the given array.</returns>
    public static T[] ShuffleArray<T>(in T[] original)
    {
        var copy = new T[original.Length];
        Array.Copy(original, copy, original.Length);

        for (int i = 0; i < copy.Length; i++)
        {
            int j = UnityEngine.Random.Range(0, copy.Length);

            (copy[i], copy[j]) = (copy[j], copy[i]);
        }

        return copy;
    }

    public void LoadAssignmentUI()
    {
        SceneManager.LoadScene(_assignmentSceneIndex);
    }
}

[System.Serializable]
public struct Question
{
    [TextArea]
    public string QuestionText;
    public Sprite QuestionSprite;
    public Answer[] Answers;
}

[System.Serializable]
public struct Answer
{
    [TextArea]
    public string AnswerText;
    public Sprite AnswerSprite;
    public bool IsCorrect;
}
