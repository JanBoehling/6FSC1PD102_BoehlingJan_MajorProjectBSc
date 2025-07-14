using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    
    [field: SerializeField] public RectTransform DetailsPrefab { get; private set; }

    [field: SerializeField] public List<Milestone> Milestones { get; private set; }
}
