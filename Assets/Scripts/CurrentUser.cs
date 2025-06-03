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

    public static void RaiseXP(int value) => Data.XP += (uint)value;
    public static void RaiseStreak() => Data.Streak++;
    public static void ResetStreak() => Data.Streak = 0;
}