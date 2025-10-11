using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class AchievementManager : MonoSingleton<AchievementManager>
{
    [field: SerializeField] public Achievement[] Achievements { get; private set; }

    [SerializeField] private Image[] _achievementImageSlots;

    private List<Achievement> _completedAchievements;

    private bool _hasFetchedAchievements;

    private void Start()
    {
        FetchAchievementStates();
    }

    private void OnEnable()
    {
        if (!_hasFetchedAchievements) return;
        RefreshAchievementIcons();
    }

    /// <summary>
    /// Creates a list with all the completed achievements from the given list
    /// </summary>
    /// <param name="achievements">Complete achievement pool</param>
    /// <returns>A sorted list with all completed achievements</returns>
    private static List<Achievement> GetAllCompletedAchievements(Achievement[] achievements)
    {
        return achievements
            .Where((achievement) => achievement.IsCompleted)
            .OrderByDescending(achievement => achievement.CompletionTimestamp) // Descending or ascending?
            .ToList();
    }

    private void DisplayRecentAchievements()
    {
        for (int i = 0; i < _achievementImageSlots.Length; i++)
        {
            _achievementImageSlots[i].sprite = _completedAchievements[i].AchievementIcon;
        }
    }

    public void RefreshAchievementIcons()
    {
        _completedAchievements = GetAllCompletedAchievements(Achievements);

        DisplayRecentAchievements();
    }

    /// <summary>
    /// Fetches all achievements from DB
    /// </summary>
    private void FetchAchievementStates()
    {
        DB.Instance.Query(achievementCompletionState =>
        {
            DB.Instance.Query(achievementLinks =>
            {
                for (int i = 0; i < achievementCompletionState.Length; i++)
                {
                    int link = int.Parse(achievementLinks[i]);
                    bool state = int.Parse(achievementCompletionState[i]) != 0;

                    Achievements[link].SetCompletionState(state);
                }

                _hasFetchedAchievements = true;

            }, $"SELECT achievementLink FROM AchievementProgress INNER JOIN UserData ON UserData.userID = AchievementProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY achievementLink");

        }, $"SELECT isCompleted FROM UserData INNER JOIN AchievementProgress ON UserData.userID = AchievementProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY achievementLink");
    }
}
