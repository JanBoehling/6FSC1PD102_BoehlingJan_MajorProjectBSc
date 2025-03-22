using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    public string Title;

    public List<Assignment> Assignments;
}

[System.Serializable]
public struct Assignment
{

}
