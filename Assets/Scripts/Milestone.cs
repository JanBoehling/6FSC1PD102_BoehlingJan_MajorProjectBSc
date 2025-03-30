using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct Milestone
{
    public bool IsCompleted;
    public bool IsVideo;

    public string Title;
    public Sprite Icon;

    public VideoClip Video;

    public int XP;

    public Assignment[] Assignments;
}
