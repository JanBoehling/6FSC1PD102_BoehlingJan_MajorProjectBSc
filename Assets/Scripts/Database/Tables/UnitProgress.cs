[System.Serializable]
public class UnitProgress
{
    public uint UnitID;
    public int UnitLink;
    public bool IsCompleted;
    public uint UserID;

    public UnitProgress(uint unitID, int unitLink, bool isCompleted, uint userID)
    {
        UnitID = unitID;
        UnitLink = unitLink;
        IsCompleted = isCompleted;
        UserID = userID;
    }
}
