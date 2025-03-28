using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    public bool IsCompleted;

    public string Title;

    public List<Milestone> Milestones;
}

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
