using System.Collections;
using UnityEngine;

public class UnitCarousel : MonoSingleton<UnitCarousel>
{
    [Tooltip("Debug: Directly move the carousel")]
    [SerializeField] private float _unitPosition = 0f;

    [Tooltip("This value should correspond to the spacing of the unit objects in the world")]
    [SerializeField] private float _spacing = 2f;

    [Tooltip("The speed of the swipe animation")]
    [SerializeField] private float _animationSpeed = 1f;

    private Coroutine _currentAnimation;

    public int UnitIndex => Mathf.RoundToInt(_unitPosition);

    private void OnValidate()
    {
        _unitPosition = Mathf.Clamp(_unitPosition, 0f, transform.childCount - 1f);

        MoveCarousel();
    }

    private void Update()
    {
        MoveCarousel();
    }

    private void MoveCarousel()
    {
        var pos = transform.position;
        pos.x = -_unitPosition * transform.localScale.x * _spacing;
        pos.z = _unitPosition * transform.localScale.x * _spacing;
        transform.position = pos;
    }

    public void SwipeCarousel(float direction)
    {
        if (_currentAnimation != null) return;
        else if ((_unitPosition == 0f && direction < 0f) || (_unitPosition == transform.childCount - 1f && direction > 0f)) return;

        _currentAnimation = StartCoroutine(SwipeCarouselCO(Mathf.Sign(direction)));
    }

    private IEnumerator SwipeCarouselCO(float direction)
    {
        // Gets target index
        float targetPos = _unitPosition + direction;

        while (true)
        {
            // Increment position
            _unitPosition += Time.deltaTime * _animationSpeed * direction;

            // Clamps position to target position
            _unitPosition = direction > 0f ? Mathf.Clamp(_unitPosition, _unitPosition, targetPos) : Mathf.Clamp(_unitPosition, targetPos, _unitPosition);

            // Breaks out of loop if animation is near end
            if (direction < 0f && (_unitPosition - Mathf.Floor(_unitPosition)) <= .01f) break;
            if (direction > 0f && (Mathf.Ceil(_unitPosition) - _unitPosition) <= .01f) break;

            yield return null;
        }

        // Resets position to target position
        _unitPosition = targetPos;

        _currentAnimation = null;
    }

    public UnitData GetCurrentUnitData()
    {
        var currentUnitObject = transform.GetChild(UnitIndex);
        var currentUnit = currentUnitObject.GetComponent<Unit>();
        var data = currentUnit.GetUnitData();
        
        return data;
    }
}
