using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;

    public UnitData GetUnitData() => _unitData;
}
