using UnityEngine;

public abstract class Assignment : ScriptableObject
{
    [SerializeField] private GameObject _assignmentUIPrefab;

    public void LoadAssignmentUI()
    {
        AssignmentController.Instance.Enable(_assignmentUIPrefab);
    }
}

[System.Serializable]
public struct Answer
{
    public string AnswerText;
    public Sprite AnswerSprite;
    public bool IsCorrect;
}
