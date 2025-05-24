using UnityEngine;

[System.Serializable]
public class Milestone
{
    public string Title;
    public Sprite Icon;
    [Space]
    public int XP;
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
                if (CompletionTracker.Instance.GetAssignmentCompletionState(assignmentID)) completedAssignments++;
            }

            return completedAssignments;
        } 
    }

    //public int VideoCount
    //{
    //    get
    //    {
    //        int videoCount = 0;

    //        foreach (uint assignmentID in Assignments)
    //        {
    //            if (CompletionTracker.Instance.GetAssignmentByID(assignmentID) is VideoAssignment) videoCount++;
    //        }

    //        return videoCount;
    //    }
    //}

    //public int CompletedVideos
    //{
    //    get
    //    {
    //        int completedVideos = 0;

    //        foreach (uint assignmentID in Assignments)
    //        {
    //            if (CompletionTracker.Instance.GetAssignmentByID(assignmentID) is VideoAssignment && CompletionTracker.Instance.AssignmentCompletionState[assignmentID]) completedVideos++;
    //        }

    //        return completedVideos;
    //    }
    //}

    public bool IsCompleted => CompletedAssignments / Assignments.Length >= CompletionPercentThreshold;
}
