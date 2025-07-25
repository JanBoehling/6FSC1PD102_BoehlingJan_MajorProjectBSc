using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class UnitCarousel : MonoBehaviour
{
    public float UnitPosition { get; private set; } = 0f;
    public int UnitIndex => Mathf.RoundToInt(UnitPosition);

    [Tooltip("The speed of the swipe animation")]
    [SerializeField] private float _animationSpeed = 1f;

    [SerializeField] private UnityEvent _onChangeCurrentUnit;

    [SerializeField] private UnityEngine.UI.Button _leftSwipeButton;
    [SerializeField] private UnityEngine.UI.Button _rightSwipeButton;

    private Coroutine _currentAnimation;

    private static UnitCarousel _instance;

    private Func<bool> pageLeftMovePredicate;
    private Func<bool> pageRightMovePredicate;

    private DeviceOrientationDetector _orientation;

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

        _orientation = GetComponent<DeviceOrientationDetector>();
    }

    private void Start()
    {
        UpdateInteractivity();
    }

    private void Update()
    {
        MoveCarousel();
    }

    public void SetUnitPositions()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            // Set offset position
            var offset = i * _orientation.ScreenSizeInWorldSpace.width;

            // Apply new local position to child
            child.localPosition = new(offset, child.localPosition.y, -offset);

            // Activate animation
            if (child.TryGetComponent<SineBounce>(out var sineBounce))
            {
                sineBounce.Init();
            }
        }
    }

    private void UpdateInteractivity()
    {
        _leftSwipeButton.interactable = pageLeftMovePredicate.Invoke();
        _rightSwipeButton.interactable = pageRightMovePredicate.Invoke();
    }

    private void MoveCarousel()
    {
        var pos = transform.position;
        pos.x = -UnitPosition * transform.localScale.x * _orientation.ScreenSizeInWorldSpace.width;
        pos.z = UnitPosition * transform.localScale.x * _orientation.ScreenSizeInWorldSpace.width;
        transform.position = pos;
    }

    public void SwipeCarousel(float direction)
    {
        if (_currentAnimation != null) return;
        else if ((UnitPosition == 0f && direction < 0f) || (UnitPosition == transform.childCount - 1f && direction > 0f)) return;

        _currentAnimation = StartCoroutine(SwipeCarouselCO(Mathf.Sign(direction)));
    }

    private IEnumerator SwipeCarouselCO(float direction)
    {
        // Gets target index
        float targetPos = UnitPosition + direction;

        while (true)
        {
            // Increment position
            UnitPosition += Time.deltaTime * _animationSpeed * direction;

            // Clamps position to target position
            UnitPosition = direction > 0f ? Mathf.Clamp(UnitPosition, UnitPosition, targetPos) : Mathf.Clamp(UnitPosition, targetPos, UnitPosition);

            // Breaks out of loop if animation is near end
            if (direction < 0f && (UnitPosition - Mathf.Floor(UnitPosition)) <= .01f) break;
            if (direction > 0f && (Mathf.Ceil(UnitPosition) - UnitPosition) <= .01f) break;

            yield return null;
        }

        // Resets position to target position
        UnitPosition = targetPos;

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

#if UNITY_EDITOR
[CustomEditor(typeof(UnitCarousel))]
public class UnitCarouselEditor : Editor
{
    private UnitCarousel _unitCarousel;

    private void OnEnable()
    {
        _unitCarousel = (UnitCarousel)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField($"Current Unit: {_unitCarousel.UnitPosition}");

        if (!Application.isPlaying) return;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("<"))
        {
            _unitCarousel.SwipeCarousel(-1);
        }

        else if (GUILayout.Button(">"))
        {
            _unitCarousel.SwipeCarousel(1);
        }

        EditorGUILayout.EndHorizontal();
    }
    public override bool RequiresConstantRepaint() => true;
}
#endif
