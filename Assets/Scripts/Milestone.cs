using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class Milestone
{
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

    public int VideoCount
    {
        get
        {
            int videoCount = 0;

            foreach (var assignment in Assignments)
            {
                if (assignment is VideoAssignment) videoCount++;
            }

            return videoCount;
        }
    }

    public int CompletedVideos
    {
        get
        {
            int completedVideos = 0;

            foreach (var assignment in Assignments)
            {
                if (assignment is VideoAssignment && assignment.IsCompleted) completedVideos++;
            }

            return completedVideos;
        }
    }

    public bool IsCompleted => CompletedAssignments / Assignments.Length >= CompletionPercentThreshold;
}
