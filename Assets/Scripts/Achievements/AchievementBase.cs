using System;
using UnityEngine;

public abstract class AchievementBase : ScriptableObject
{
    /// <summary>
    /// The time of completion. 
    /// </summary>
    public DateTime CompletionTimestamp { get; protected set; }

    [SerializeField] protected Sprite _achievementIcon;
    [SerializeField] private int _tier = 1;

    protected bool IsCompleted = false;

    /// <summary>
    /// Checks if the condition for completing the achievement has been met.<br/>
    /// If so, saves the time of completion.
    /// </summary>
    /// <returns>Whether the achievement has been completed or not</returns>
    public bool HasCompleted()
    {
        if (IsCompleted) return IsCompleted;

        if (!CompletionCondition()) return false;
        
        IsCompleted = true;
        CompletionTimestamp = DateTime.Now;
        return IsCompleted;
    }

    /// <summary>
    /// This method can be overridden to calculate if the achievement has been completed or not
    /// </summary>
    /// <returns>Whether the condition for completion has been met or not</returns>
    protected abstract bool CompletionCondition();

    /// <returns>The icon of this achievement</returns>
    public Sprite GetAchievementIcon() => _achievementIcon;
}
