[System.Serializable]
public class AssignmentProgress
{
    public uint AssignmentID;
    public int AssignmentLink;
    public bool IsCompleted;
    public uint UserID;

    public AssignmentProgress(uint assignmentID, int assignmentLink, bool isCompleted, uint userID)
    {
        AssignmentID = assignmentID;
        AssignmentLink = assignmentLink;
        IsCompleted = isCompleted;
        UserID = userID;
    }
}
