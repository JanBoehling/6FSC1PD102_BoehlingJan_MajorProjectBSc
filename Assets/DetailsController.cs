using UnityEngine;
using UnityEngine.UI;

public class DetailsController : MonoBehaviour
{
    [SerializeField] private RectTransform _viewport;
    [SerializeField] private ScrollRect _scrollRect;

    private void OnEnable()
    {
        var unitData = UnitCarousel.GetUnitCarousel().GetCurrentUnitData();

        if (!unitData.DetailsPrefab) return;

        var content = Instantiate(unitData.DetailsPrefab, _viewport);
        _scrollRect.content = content;
    }
}
