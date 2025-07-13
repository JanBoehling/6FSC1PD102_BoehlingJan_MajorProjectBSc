using UnityEngine;

public class UnitOverviewController : MonoBehaviour, IToggleVisibility
{
    [SerializeField] private UnitListItem _unitListItemPrefab;
    [SerializeField] private Transform _unitListContainer;

    private TopIslandController _topIsland;

    private void Awake()
    {
        _topIsland = FindFirstObjectByType<TopIslandController>();
    }

    /// <summary>
    /// Is called when the unit overview is opened
    /// </summary>
    public void ActivateOverview()
    {
        transform.GetChild(0).gameObject.SetActive(true);

        var unitData = UnitCarousel.GetUnitCarousel().GetCurrentUnitData();

        _topIsland.DisplayTitle(unitData.Title);

        foreach (var milestone in unitData.Milestones)
        {
            var listItem = Instantiate(_unitListItemPrefab, _unitListContainer);
            listItem.Init(milestone);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    
}
