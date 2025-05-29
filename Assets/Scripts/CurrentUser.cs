public static class CurrentUser
{
    public static UserData Data { get; private set; }

    public static uint Streak => Data.Streak;
    public static uint XP => Data.XP;

    public static void SetUser(UserData user)
    {
        if (Data is not null)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError($"User with name {user.UserName} is already logged in.");
#endif
            return;
        }

        Data = user;
    }

    public static void RaiseXP(int value) => Data.XP += (uint)value;
    public static void RaiseStreak() => Data.Streak++;
    public static void ResetStreak() => Data.Streak = 0;
}