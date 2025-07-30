using UnityEngine;

[System.Serializable]
public class Milestone
{
    public string Title;
    public string Notes;
    public Sprite Icon;
    [Space]
    public uint XP;
    [Space]
    public uint[] Assignments;
    [Space]
    public float CompletionPercentThreshold;

    public int CompletedAssignments
    {
        get
        {
            int completedAssignments = 0;

            foreach (var assignmentID in Assignments)
            {
                if (UnitAndAssignmentManager.Instance.GetAssignmentCompletionState(assignmentID)) completedAssignments++;
            }

            return completedAssignments;
        } 
    }

    public bool IsCompleted => CompletedAssignments / Assignments.Length >= CompletionPercentThreshold;
}
