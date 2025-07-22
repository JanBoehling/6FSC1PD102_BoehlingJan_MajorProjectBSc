using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Instantiates a prefab when activated. The prefab can be determined in the implementing class
/// </summary>
public abstract class TextWindowController : MonoBehaviour, IToggleVisibility
{
    [SerializeField] private RectTransform _viewport;
    [SerializeField] private ScrollRect _scrollRect;

    protected abstract Func<UnitData, RectTransform> ObjectToInstantiate { get; }

    private TopIslandController _topIsland;

    private UnitData _currentUnitData;

    private void Awake()
    {
        _topIsland = FindFirstObjectByType<TopIslandController>();
    }

    protected virtual void OnEnable()
    {
        _currentUnitData = UnitCarousel.GetUnitCarousel().GetCurrentUnitData();

        _topIsland.DisplayTitle(_currentUnitData.Title);

        var obj = ObjectToInstantiate?.Invoke(_currentUnitData);

        if (!obj) return;

        var content = Instantiate(obj, _viewport);
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
