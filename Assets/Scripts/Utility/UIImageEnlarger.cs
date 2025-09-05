using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIImageEnlarger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField, Tooltip("The offset of the image origin when enlarged")] private Vector2 _offset = Vector2.zero;
    [SerializeField, Tooltip("The percentage, the image is enlarged by")] private float _enlargementPercent = 2f;
    [Space]
    [SerializeField] private UnityEvent _onToggleEvent;

    private bool _isLarge = false;

    private RectTransform _transform;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _onToggleEvent.AddListener(() =>
        {
            var parent = transform.parent;
            for (int i = 1; i < parent.childCount; i++)
            {
                if (!parent.GetChild(i).TryGetComponent<VisibilityToggle>(out var toggle)) continue;
                
                toggle.ToggleVisibility();
            }
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isLarge = !_isLarge;

        if (_isLarge)
        {
            _transform.localScale *= _enlargementPercent;
            _transform.pivot += _offset;
        }
        else
        {
            _transform.localScale /= _enlargementPercent;
            _transform.pivot -= _offset;
        }

        _onToggleEvent?.Invoke();
    }
}
