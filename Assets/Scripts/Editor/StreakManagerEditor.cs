using System;
using UnityEngine;
using UnityEditor;

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

    private void OnEnable() => _streakManager = (StreakManager)target;

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

    /// <summary>
    /// Converts the given data numbers to a DateTime object
    /// </summary>
    /// <param name="label">The label of the int sliders</param>
    /// <returns>A DateTime object consiting of the selected year, month and day</returns>
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
