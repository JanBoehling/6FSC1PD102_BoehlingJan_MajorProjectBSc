using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwipeController : MonoSingleton<SwipeController>, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
    [Tooltip("This event is called when the user lifts his finger after a swipe")]
    [SerializeField] private UnityEvent<float> _onDragEndEventHandler;

#if UNITY_EDITOR
    // Access for editor script
    public UnityEvent<float> OnDragEndEventHandler => _onDragEndEventHandler;
#endif

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
}
