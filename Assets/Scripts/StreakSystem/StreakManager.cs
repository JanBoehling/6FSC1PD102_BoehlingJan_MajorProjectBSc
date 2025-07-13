using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    private DateTime _lastStreakRefresh;

    private bool IsStreakRefreshed => _lastStreakRefresh == DateTime.Today;

    private void Start()
    {
        if (!IsStreakRefreshed) UpdateStreak();
    }

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

    public void OverrideLastRefreshDate(DateTime date) => _lastStreakRefresh = date;
}

#if UNITY_EDITOR
[CustomEditor(typeof(StreakManager))]
public class StreakManagerEditor : Editor
{
    private StreakManager _streakManager;

    private int _newTodayD = DateTime.Today.Day;
    private int _newTodayM = DateTime.Today.Month;
    private int _newTodayY = DateTime.Today.Year;

    private int _newLastRefreshD = DateTime.Today.Day;
    private int _newLastRefreshM = DateTime.Today.Month;
    private int _newLastRefreshY = DateTime.Today.Year;

    private void OnEnable()
    {
        _streakManager = (StreakManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (!_streakManager.UseDebugDay) return;

        _streakManager.CustomDate = GetCustomDate(ref _newTodayY, ref _newTodayM, ref _newTodayD, "Override Today");
        _streakManager.OverrideLastRefreshDate(GetCustomDate(ref _newLastRefreshY, ref _newLastRefreshM, ref _newLastRefreshD, "Override Last Refresh Date"));

        EditorGUILayout.Space();

        if (GUILayout.Button("Refresh Streak"))
        {
            _streakManager.UpdateStreak();
        }
    }

    private DateTime GetCustomDate(ref int year, ref int month, ref int day, string label = "")
    {
        EditorGUILayout.Space();
        if (!string.IsNullOrWhiteSpace(label)) EditorGUILayout.LabelField(label);

        day = EditorGUILayout.IntSlider("D", day, 1, 31);
        month = EditorGUILayout.IntSlider("M", month, 1, 12);
        year = EditorGUILayout.IntSlider("Y", year, 1, DateTime.Today.Year);

        return new(year, month, day);
    }
}
#endif

[System.Serializable]
public struct CustomStreakMessage
{
    public int DayStreak;
    public string Message;

    public override string ToString() => Message;
}
