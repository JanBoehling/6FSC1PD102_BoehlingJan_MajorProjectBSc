using System;
using UnityEngine;

[System.Serializable]
public class Achievement
{
    /// <summary>
    /// The time of completion of this achievement
    /// </summary>
    public DateTime CompletionTimestamp { get; protected set; }

    /// <summary>
    /// The state of completion of this achievement
    /// </summary>
    public bool IsCompleted { get; protected set; } = false;

    [field: SerializeField, Tooltip("The display name of the achievement")] public string AchievementName { get; protected set; }
    [field: SerializeField, Tooltip("The display image of the acheivement")] public Sprite AchievementIcon { get; protected set; }

    public void SetCompletionState(bool completionState = true)
    {
        IsCompleted = completionState;
    }
}
