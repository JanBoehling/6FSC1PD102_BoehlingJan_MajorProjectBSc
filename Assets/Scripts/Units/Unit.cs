using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private uint _unitID;

    
    /// <returns>The data of the unit with the given ID</returns>
    public UnitData GetUnitData() => UnitAndAssignmentManager.Instance.GetUnitByID(_unitID);
}
