using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProteintinderAssignmentController : AssignmentControllerBase<ProteintinderAssignment>
{
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

    protected override void Start()
    {
        base.Start();

#if UNITY_EDITOR
        if (UseDebugAssignment)
        {
            RuntimeDataHolder.CurrentMilestone = new()
            {
                Assignments = new[] { AssignmentID }
            };
        }
#endif
        AssignmentID = RuntimeDataHolder.CurrentMilestone.Assignments[0];
        
        Init(AssignmentID);
    }

    /// <summary>
    /// Gets the assignment data and prepares the assignment UI
    /// </summary>
    /// <param name="assignmentID">The ID of the assignment</param>
    public override void Init(uint assignmentID)
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

    /// <summary>
    /// This method is called when the user swipes the screen
    /// </summary>
    /// <param name="direction">The direction of the swipe. -1 is left and +1 is right</param>
    public void OnSwipe(float direction)
    {
        if (_currentAnimation != null) return;

        _currentAnimation = StartCoroutine(OnSwipeCO(Mathf.Sign(direction)));
    }

    /// <summary>
    /// Moves the topmost image from the image stack to the given direction and increments the current image index
    /// </summary>
    /// <param name="direction">The direction of the swipe. -1 is left and +1 is right</param>
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
