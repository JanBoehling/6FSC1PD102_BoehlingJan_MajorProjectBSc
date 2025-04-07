using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class Milestone
{
    public bool IsVideo;
    public VideoClip Video;
    [Space]
    public string Title;
    public Sprite Icon;
    [Space]
    public int XP;
    [Space]
    public Assignment[] Assignments;
    [Space]
    public float CompletionPercentThreshold;

    public int CompletedAssignments
    {
        get
        {
            int completedAssignments = 0;

            foreach (var assignment in Assignments)
            {
                if (assignment.IsCompleted) completedAssignments++;
            }

            return completedAssignments;
        } 
    }

    public bool IsCompleted => CompletedAssignments / Assignments.Length < CompletionPercentThreshold;
}
