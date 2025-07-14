using UnityEngine;

public class DetailsController : MonoBehaviour
{
    private void OnEnable()
    {
        var unitData = UnitCarousel.GetUnitCarousel().GetCurrentUnitData();


    }
}
