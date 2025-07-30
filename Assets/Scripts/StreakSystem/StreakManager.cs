using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class StreakManager : MonoBehaviour
{
    public uint DayStreak => CurrentUser.Streak;

    [SerializeField] private TMP_Text _streakText;
    [SerializeField] private TMP_Text _streakDayText;

    [SerializeField, Tooltip("Use %s as a placeholder for the streak.")] private string[] _streakMessages;
    [SerializeField, Tooltip("Use %s as a placeholder for the streak.")] private CustomStreakMessage[] _customStreakMessages;

    [field:SerializeField] public bool UseDebugDay { get; private set; }
    public DateTime CustomDate { get; set; }

    private const string StreakNumberPlaceholder = "%s";

    private bool IsStreakRefreshed => _lastStreakRefresh == DateTime.Today;

    private DateTime _lastStreakRefresh;

    private void Start()
    {
        if (!IsStreakRefreshed) UpdateStreak();
    }

    /// <summary>
    /// Updates the current streak. Raises it by one if the login was consecutive or resets it to one if not
    /// </summary>
    public void UpdateStreak()
    {
        var today = UseDebugDay ? CustomDate : DateTime.Today;
        var dayAfterLastRefresh = _lastStreakRefresh.AddDays(1);

        if (today == _lastStreakRefresh) return;

        if (today == dayAfterLastRefresh)
        {
            CurrentUser.RaiseStreak();
            _lastStreakRefresh = today;
        }
        else CurrentUser.ResetStreak();
    }

    /// <summary>
    /// Updates the streak display text element to the current streak
    /// </summary>
    public void UpdateDisplay()
    {
        var customMessages = new List<string>();

        foreach (var item in _customStreakMessages)
        {
            if (item.DayStreak != DayStreak) continue;

            customMessages.Add(item.Message);
        }

        bool useCustomMessage = customMessages.Count > 0;

        int randomMessageIndex = Random.Range(0, useCustomMessage ? customMessages.Count : _streakMessages.Length);
        string message = useCustomMessage ? customMessages[randomMessageIndex] : _streakMessages[randomMessageIndex];

        message = message.Replace(StreakNumberPlaceholder, DayStreak.ToString());

        _streakText.text = message;

        _streakDayText.text = DayStreak.ToString();
    }

    /// <summary>
    /// Enables debug tools to override the last login date
    /// </summary>
    /// <param name="date">The date that the last login date should be overridden with</param>
    public void OverrideLastRefreshDate(DateTime date) => _lastStreakRefresh = date;
}

[System.Serializable]
public struct CustomStreakMessage
{
    public int DayStreak;
    public string Message;

    public override readonly string ToString() => Message;
}
