public static class CurrentUser
{
    public static UserData Data { get; private set; }

    public static uint Streak => Data.Streak;
    public static uint XP => Data.XP;
    public static uint UserID => Data.UserID;
    public static uint ProfilePictureIndex => Data.ProfilePictureIndex;

    public static void SetUser(UserData user)
    {
        Data = user;
    }

    public static void SyncUser()
    {
        var userDataRaw = new System.Collections.Generic.KeyValuePair<string, string>[]
        {
            new (nameof(Data.UserID), Data.UserID.ToString()),
            new (nameof(Data.UserName), Data.UserName.ToString()),
            new (nameof(Data.Password), Data.Password.ToString()),
            new (nameof(Data.Streak), Data.Streak.ToString()),
            new (nameof(Data.XP), Data.XP.ToString()),
            new (nameof(Data.ProfilePictureIndex), Data.ProfilePictureIndex.ToString())
        };

        DB.Instance.UpdateQuery(null, "UserData", userDataRaw);
    }

    public static void RaiseXP(uint value)
    {
        Data.XP += value;
        DB.Instance.UpdateQuery(null, "UserData", "XP", XP);
    }
    
    public static void RaiseStreak()
    {
        Data.Streak++;
        DB.Instance.UpdateQuery(null, "UserData", "Streak", Streak);
    }

    public static void ResetStreak()
    {
        Data.Streak = 1;
        DB.Instance.UpdateQuery(null, "UserData", "Streak", Streak);
    }

    public static void SetProfilePicture(System.Action callback, uint profilePictureIndex)
    {
        Data.ProfilePictureIndex = profilePictureIndex;
        DB.Instance.UpdateQuery((_) => callback?.Invoke(), "UserData", "profilePictureIndex", profilePictureIndex);
    }
}