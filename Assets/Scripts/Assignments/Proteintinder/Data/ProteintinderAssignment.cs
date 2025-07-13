using UnityEngine;

[CreateAssetMenu]
public class ProteintinderAssignment : AssignmentDataBase
{
    [field: SerializeField] public ProteintinderQuestion[] ImageStack { get; private set; }
    [field: SerializeField] public string QuestionText { get; private set; }
}

[System.Serializable]
public class ProteintinderQuestion
{
    public Sprite Image;
    public bool IsCorrect;
}
