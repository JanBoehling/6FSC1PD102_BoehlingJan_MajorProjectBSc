using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class StreakManager : MonoSingleton<StreakManager>
{
    [field:SerializeField, HideInInspector] public int DayStreak { get; private set; }
    [SerializeField] private TMP_Text _streakText;

    [SerializeField, Tooltip("Use {X} as a placeholder for the streak.")] private string[] _streakMessages;
    [SerializeField, Tooltip("Use {X} as a placeholder for the streak.")] private CustomStreakMessage[] _customStreakMessages;

    private const string streakNumberPlaceholder = "{X}";

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

        message = message.Replace(streakNumberPlaceholder, DayStreak.ToString());

        _streakText.text = message;
    }
}

[System.Serializable]
public struct CustomStreakMessage
{
    public int DayStreak;
    public string Message;

    public override string ToString() => Message;
}
