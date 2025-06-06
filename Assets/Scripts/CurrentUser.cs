public static class CurrentUser
{
    public static UserData Data { get; private set; }

    public static uint Streak => Data.Streak;
    public static uint XP => Data.XP;
    public static uint UserID => Data.UserID;

    public static void SetUser(UserData user)
    {
        Data = user;
    }

    public static async void SyncUser()
    {
        var userDataRaw = new System.Collections.Generic.KeyValuePair<string, string>[]
        {
            new (nameof(Data.UserID), Data.UserID.ToString()),
            new (nameof(Data.UserName), Data.UserName.ToString()),
            new (nameof(Data.Password), Data.Password.ToString()),
            new (nameof(Data.Streak), Data.Streak.ToString()),
            new (nameof(Data.XP), Data.XP.ToString())
        };

        await DB.Update("UserData", userDataRaw);
    }

    public static async void RaiseXP(uint value)
    {
        Data.XP += value;
        await DB.Update("UserData", "XP", XP.ToString());
    }
    
    public static async void RaiseStreak()
    {
        Data.Streak++;
        await DB.Update("UserData", "Streak", Streak.ToString());
    }
    public static async void ResetStreak()
    {
        Data.Streak = 0;
        await DB.Update("UserData", "Streak", Streak.ToString());
    }
}