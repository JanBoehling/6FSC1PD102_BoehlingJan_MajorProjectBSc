using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private uint _unitID;

    public UnitData GetUnitData() => UnitAndAssignmentManager.Instance.GetUnitByID(_unitID);
}
