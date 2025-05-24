using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private uint _unitID;

    public UnitData GetUnitData() => CompletionTracker.Instance.GetUnitByID(_unitID);
}
