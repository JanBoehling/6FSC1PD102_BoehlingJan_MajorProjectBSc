[System.Serializable]
public class UserData
{
    public uint UserID;
    public string UserName;
    public string Password;
    public uint Streak;
    public uint XP;

    public UserData(uint userID, string userName, string password, uint streak, uint xp)
    {
        UserID = userID;
        UserName = userName;
        Password = password;
        Streak = streak;
        XP = xp;
    }
}
