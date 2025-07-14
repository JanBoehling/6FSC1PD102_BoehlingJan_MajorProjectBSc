using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler, IPointerClickHandler
{
    [Tooltip("This event is invoked when the user lifts his finger after a swipe")]
    [SerializeField] private UnityEvent<float> _onDragEndEventHandler;

    [Tooltip("This event is invoked when the user clicks or taps")]
    [SerializeField] private UnityEvent _onClickEventHandler;

    public UnityEvent<float> OnDragEndEventHandler => _onDragEndEventHandler;

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _onDragEndEventHandler?.Invoke(-Mathf.Sign(eventData.delta.x));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClickEventHandler?.Invoke();
    }
}
