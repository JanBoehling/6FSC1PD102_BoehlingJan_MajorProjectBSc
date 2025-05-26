[System.Serializable]
public class User
{
    public uint UserID;
    public string UserName;
    public string Password;
    public uint Streak;
    public uint XP;

    public User(uint userID, string userName, string password, uint streak, uint xp)
    {
        UserID = userID;
        UserName = userName;
        Password = password;
        Streak = streak;
        XP = xp;
    }
}
