using UnityEngine;

public class UnitOverviewController : MonoBehaviour
{
    /// <summary>
    /// Is called when the unit overview is opened
    /// </summary>
    public void ActivateOverview()
    {
        transform.GetChild(0).gameObject.SetActive(true);

        var unitData = UnitCarousel.Instance.GetCurrentUnitData();

        TopIslandController.Instance.DisplayTitle(unitData.Title);
    }
}
