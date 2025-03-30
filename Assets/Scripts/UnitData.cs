using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    public bool IsCompleted;

    public string Title;

    public List<Milestone> Milestones;
}
