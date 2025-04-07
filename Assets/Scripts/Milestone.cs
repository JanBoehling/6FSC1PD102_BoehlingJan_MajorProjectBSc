using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct Milestone
{
    public bool IsCompleted;
    public bool IsVideo;

    public string Title;
    public Sprite Icon;

    public VideoClip Video;

    public int XP;

    public Assignment[] Assignments;

    public readonly int CompletedAssignments
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
}
