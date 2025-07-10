using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnitCarousel : MonoBehaviour
{
    [Tooltip("Debug: Directly move the carousel")]
    [SerializeField] private float _unitPosition = 0f;

    [Tooltip("This value should correspond to the spacing of the unit objects in the world")]
    [SerializeField] private float _spacing = 2f;
    private float _screenWidth;

    [Tooltip("The speed of the swipe animation")]
    [SerializeField] private float _animationSpeed = 1f;

    [SerializeField] private UnityEvent _onChangeCurrentUnit;

    [SerializeField] private UnityEngine.UI.Button _leftSwipeButton;
    [SerializeField] private UnityEngine.UI.Button _rightSwipeButton;

    private Coroutine _currentAnimation;

    public int UnitIndex => Mathf.RoundToInt(_unitPosition);

    private static UnitCarousel _instance;

    private Func<bool> pageLeftMovePredicate;
    private Func<bool> pageRightMovePredicate;

    public static UnitCarousel GetUnitCarousel()
    {
        if (_instance != null) return _instance;

        var carousel = FindObjectsByType<UnitCarousel>(FindObjectsSortMode.None);

        if (carousel.Length < 1) Debug.LogError("Error: Could not find UnitCarousel in scene!");
        else if (carousel.Length > 1) Debug.LogError("Error: Multiple UnitCarousels in scene!");
        else
        {
            _instance = carousel[0];
            return _instance;
        }

        return null;
    }

    private void Awake()
    {
        pageLeftMovePredicate = () => UnitIndex > 0;
        pageRightMovePredicate = () => UnitIndex < transform.childCount - 1;

        _screenWidth = Camera.main.ScreenToWorldPoint(new(FindAnyObjectByType<Canvas>().pixelRect.width, FindAnyObjectByType<Canvas>().pixelRect.height)).x;
        Debug.Log(_screenWidth);
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var offset = i * transform.localScale.x * -_screenWidth;
            transform.GetChild(i).position = new(offset, transform.position.y, -offset);

            // Activate animation
            if (transform.GetChild(i).TryGetComponent<SineBounce>(out var sineBounce))
            {
                sineBounce.Init();
            }
        }


        UpdateInteractivity();
    }

    private void OnValidate()
    {
        _unitPosition = Mathf.Clamp(_unitPosition, 0f, transform.childCount - 1f);

        MoveCarousel();
    }

    private void Update()
    {
        MoveCarousel();
    }

    private void UpdateInteractivity()
    {
        _leftSwipeButton.interactable = pageLeftMovePredicate.Invoke();
        _rightSwipeButton.interactable = pageRightMovePredicate.Invoke();
    }

    private void MoveCarousel()
    {
        var pos = transform.position;
        pos.x = -_unitPosition * transform.localScale.x * -_screenWidth;
        pos.z = _unitPosition * transform.localScale.x * -_screenWidth;
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

        UpdateInteractivity();

        _onChangeCurrentUnit?.Invoke();
    }

    public UnitData GetCurrentUnitData()
    {
        return GetUnitDataByIndex(UnitIndex);
    }

    public UnitData GetUnitDataByIndex(int index)
    {
        var currentUnitObject = transform.GetChild(index);
        var currentUnit = currentUnitObject.GetComponent<Unit>();
        var data = currentUnit.GetUnitData();
        
        return data;
    }

    public UnitData[] GetAllUnitData()
    {
        var unitData = new UnitData[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            unitData[i] = GetUnitDataByIndex(i);
        }

        return unitData;
    }
}
