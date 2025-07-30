using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private List<AchievementBase> _achievements;
    [SerializeField] private Image[] _achievementImageSlots;

    private readonly List<AchievementBase> _completedAchievements = new();

    private void Start()
    {
        // Sorts the completed achievements by completion date
        _completedAchievements.Sort((x, y) => DateTime.Compare(x.CompletionTimestamp, y.CompletionTimestamp));
    }

    /// <summary>
    /// Checks all achievements if one of the has been completed.
    /// </summary>
    public void CheckForAchievementCompletion()
    {
        foreach (var item in _achievements)
        {
            if (!item.HasCompleted()) continue;

            _achievements.Remove(item);
            _completedAchievements.Add(item);
        }
    }

    /// <summary>
    /// Updates the achievement image display in the profile view
    /// </summary>
    public void UpdateDisplay()
    {
        var recentAchievements = _completedAchievements.Take(_achievementImageSlots.Length).ToArray();

        for (int i = 0; i < _achievementImageSlots.Length; i++)
        {
            _achievementImageSlots[i].sprite = recentAchievements[i].GetAchievementIcon();
        }
    }
}
