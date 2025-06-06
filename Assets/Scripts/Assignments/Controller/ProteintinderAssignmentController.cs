using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProteintinderAssignmentController : MonoBehaviour
{
    [Tooltip("The speed of the swipe animation")]
    [SerializeField] private float _animationSpeed = 1f;

    [SerializeField] private Transform _imgStackContainer;
    [SerializeField] private TMP_Text _questionText;
    private Image[] _imgStack;
    private int currentImg;

    private Coroutine _currentAnimation;

    public void Init(uint assignmentID)
    {
        var assignment = CompletionTracker.Instance.GetAssignmentByID(assignmentID) as ProteintinderAssignment;

        var images = assignment.ImageStack;

        foreach (var item in images)
        {
            var img = new GameObject(nameof(item), typeof(Image)).GetComponent<Image>();
            img.transform.SetParent(_imgStackContainer, false);
            img.sprite = item.Image;
        }

        _questionText.text = assignment.QuestionText;
    }

    public void OnSwipe(float direction)
    {
        if (_currentAnimation != null) return;

        _currentAnimation = StartCoroutine(OnSwipeCO(Mathf.Sign(direction)));
    }

    private IEnumerator OnSwipeCO(float direction)
    {
        float position = _imgStack[currentImg].transform.position.x;

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

        _imgStack[currentImg].transform.position.Set(position, _imgStack[currentImg].transform.position.y, _imgStack[currentImg].transform.position.z);
        _currentAnimation = null;
    }
}
