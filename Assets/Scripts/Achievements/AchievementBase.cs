using System;
using UnityEngine;

public abstract class AchievementBase : ScriptableObject
{
    [SerializeField] protected Sprite _achievementIcon;
    [SerializeField] private int _tier = 1;

    protected bool isCompleted = false;

    public DateTime CompletionTimestamp { get; protected set; }

    public bool HasCompleted()
    {
        if (isCompleted) return isCompleted;

        if (!CompletionCondition()) return false;
        
        isCompleted = true;
        CompletionTimestamp = DateTime.Now;
        return isCompleted;
    }

    protected abstract bool CompletionCondition();

    public Sprite GetAchievementIcon() => _achievementIcon;
}
