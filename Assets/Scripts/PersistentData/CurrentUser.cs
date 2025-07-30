public static class CurrentUser
{
    public static UserData Data { get; private set; }

    public static uint Streak => Data.Streak;
    public static uint XP => Data.XP;
    public static uint UserID => Data.UserID;
    public static uint ProfilePictureIndex => Data.ProfilePictureIndex;

    /// <summary>
    /// Sets the current user data to the given data
    /// </summary>
    /// <param name="user">The data of the current user</param>
    public static void SetUser(UserData user) => Data = user;

    /// <summary>
    /// Updates the user data in the DB with the current users data
    /// </summary>
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

    /// <summary>
    /// Increases the xp by the given value and updates it in the DB
    /// </summary>
    /// <param name="value">the vlaue by which the xp should be raised</param>
    public static void RaiseXP(uint value)
    {
        Data.XP += value;
        DB.Instance.UpdateQuery(null, "UserData", "XP", XP);
    }
    
    /// <summary>
    /// Increases the streak by one and updates it in the DB
    /// </summary>
    public static void RaiseStreak()
    {
        Data.Streak++;
        DB.Instance.UpdateQuery(null, "UserData", "Streak", Streak);
    }

    /// <summary>
    /// Resets the streak to one and updates it in the DB
    /// </summary>
    public static void ResetStreak()
    {
        Data.Streak = 1;
        DB.Instance.UpdateQuery(null, "UserData", "Streak", Streak);
    }

    /// <summary>
    /// Sets the profile picture index to the given value and updates it in the DB 
    /// </summary>
    /// <param name="callback">The callback is invoked when DB update is finished</param>
    /// <param name="profilePictureIndex">The index of the profile picture the user has chosen</param>
    public static void SetProfilePicture(System.Action callback, uint profilePictureIndex)
    {
        Data.ProfilePictureIndex = profilePictureIndex;
        DB.Instance.UpdateQuery((_) => callback?.Invoke(), "UserData", "profilePictureIndex", profilePictureIndex);
    }
}