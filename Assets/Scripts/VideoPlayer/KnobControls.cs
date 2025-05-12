using UnityEngine;
using UnityEngine.EventSystems;

public class KnobControls : VideoControls, IDragHandler
{
    private RectTransform _knob;
    private RectTransform _barParent;

    private void Start()
    {
        _knob = GetComponent<RectTransform>();
        _barParent = _progressBar.transform.parent as RectTransform;
    }

    private void Update()
    {
        _knob.anchoredPosition = new Vector2(_barParent.rect.width * VideoProgress, _knob.anchoredPosition.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Wind(eventData.position);
    }
}
