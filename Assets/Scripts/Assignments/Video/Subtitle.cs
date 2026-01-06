using UnityEngine;

[System.Serializable]
public struct Subtitle
{
    [field: SerializeField, Tooltip("The sentence that should be displayed as a subtitle")] public string Text { get; private set; }
    [field: SerializeField, Tooltip("Time in seconds, when sentence should be displayed")] public float Time { get; private set; }
}
