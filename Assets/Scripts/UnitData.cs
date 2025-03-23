using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    public bool IsCompleted;

    public string Title;

    public List<Assignment> Assignments;
}

[System.Serializable]
public struct Assignment
{
    public bool IsCompleted;
    public bool IsVideo;

    public string Title;
    public Sprite Icon;

    public VideoClip Video;

    public int XP;
}
