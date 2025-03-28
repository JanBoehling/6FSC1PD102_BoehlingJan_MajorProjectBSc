using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class AchievementManager : MonoSingleton<AchievementManager>
{
    [SerializeField] private List<Achievement> _achievements;
    [SerializeField] private Image[] _achievementImageSlots;

    private List<Achievement> _completedAchievements = new();

    private void Start()
    {
        _completedAchievements.Sort((x, y) => DateTime.Compare(x.CompletionTimestamp, y.CompletionTimestamp));
    }

    public void CheckForAchievementCompletion()
    {
        foreach (var item in _achievements)
        {
            if (!item.HasCompleted()) continue;

            _achievements.Remove(item);
            _completedAchievements.Add(item);
        }
    }

    public void UpdateDisplay()
    {
        var recentAchievements = _completedAchievements.Take(_achievementImageSlots.Length).ToArray();

        for (int i = 0; i < _achievementImageSlots.Length; i++)
        {
            _achievementImageSlots[i].sprite = recentAchievements[i].GetAchievementIcon();
        }
    }
}
