using UnityEngine;
using UnityEngine.UI;

public class DetailsController : MonoBehaviour, IToggleVisibility
{
    [SerializeField] private RectTransform _viewport;
    [SerializeField] private ScrollRect _scrollRect;

    private TopIslandController _topIsland;

    private void Awake()
    {
        _topIsland = FindFirstObjectByType<TopIslandController>();
    }

    private void OnEnable()
    {
        var unitData = UnitCarousel.GetUnitCarousel().GetCurrentUnitData();

        _topIsland.DisplayTitle(unitData.Title);

        if (!unitData.DetailsPrefab) return;

        var content = Instantiate(unitData.DetailsPrefab, _viewport);
        _scrollRect.content = content;
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
