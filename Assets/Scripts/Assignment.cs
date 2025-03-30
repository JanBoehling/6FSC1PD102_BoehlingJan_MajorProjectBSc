using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Assignment : ScriptableObject
{
    [SerializeField] private int _assignmentSceneIndex;

    public void LoadAssignmentUI()
    {
        SceneManager.LoadScene(_assignmentSceneIndex);
    }
}

[System.Serializable]
public struct Answer
{
    public string AnswerText;
    public Sprite AnswerSprite;
    public bool IsCorrect;
}
