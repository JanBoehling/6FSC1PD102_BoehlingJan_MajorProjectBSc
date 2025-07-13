using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProteintinderAssignmentController : MonoBehaviour
{
    [Header("Assignment ID")]
    [SerializeField] private uint _assignmentID;
    [SerializeField] private bool _useDebugAssignment;

    [Header("Settings")]
    [Tooltip("The speed of the swipe animation")]
    [SerializeField] private float _animationSpeed = 1f;

    [Header("References")]
    [SerializeField] private Transform _imgStackContainer;
    [SerializeField] private TMP_Text _questionText;

    [Header("Debug")]
    [SerializeField] private Image[] _imgStack;
    private int _currentImg;

    private Coroutine _currentAnimation;

    private void Start()
    {
#if UNITY_EDITOR
        if (_useDebugAssignment)
        {
            RuntimeDataHolder.CurrentMilestone = new()
            {
                Assignments = new[] { _assignmentID }
            };
        }
#endif
        _assignmentID = RuntimeDataHolder.CurrentMilestone.Assignments[0];

        Init(_assignmentID);
    }

    public void Init(uint assignmentID)
    {
        var assignment = UnitAndAssignmentManager.Instance.GetAssignmentByID(assignmentID) as ProteintinderAssignment;

        var images = assignment.ImageStack;

        var imgStackList = new System.Collections.Generic.List<Image>();

        foreach (var item in images)
        {
            var img = new GameObject(nameof(item), typeof(Image)).GetComponent<Image>();
            img.transform.SetParent(_imgStackContainer, false);
            img.sprite = item.Image;
            imgStackList.Add(img);
        }

        _imgStack = imgStackList.ToArray();

        _questionText.text = assignment.QuestionText;
    }

    public void OnSwipe(float direction)
    {
        if (_currentAnimation != null) return;

        _currentAnimation = StartCoroutine(OnSwipeCO(Mathf.Sign(direction)));
    }

    private IEnumerator OnSwipeCO(float direction)
    {
        float position = _imgStack[_currentImg].transform.position.x;

        // Gets target index
        float targetPos = position + direction;

        while (true)
        {
            // Increment position
            position += Time.deltaTime * _animationSpeed * direction;

            // Clamps position to target position
            position = direction > 0f ? Mathf.Clamp(position, position, targetPos) : Mathf.Clamp(position, targetPos, position);

            // Breaks out of loop if animation is near end
            if (direction < 0f && (position - Mathf.Floor(position)) <= .01f) break;
            if (direction > 0f && (Mathf.Ceil(position) - position) <= .01f) break;

            yield return null;
        }

        // Resets position to target position
        position = targetPos;

        _imgStack[_currentImg].transform.position.Set(position, _imgStack[_currentImg].transform.position.y, _imgStack[_currentImg].transform.position.z);
        _currentImg++;
        _currentAnimation = null;
    }
}
