using UnityEngine;
using System.Collections;
using System;

public class PageMoveController : MonoSingleton<PageMoveController>
{
    public Action OnMoveAnimationBeginAction { get; set; }
    public Action OnMoveAnimationFinishedAction { get; set; }

    public int CurrentPage { get; private set; }

    [SerializeField, Tooltip("The duration of the animation in seconds")] private float _animationDuration = 1f;
    [SerializeField] private AnimationCurve _pageMoveAnimationCurve;
    [SerializeField, Tooltip("Optional: Plays audio when page moves.")] private AudioPlayer _moveAudioPlayer;

    private Coroutine _pageMoveAnimation;

    /// <summary>
    /// Increments the current page index and starts the page move animation
    /// </summary>
    public void MovePage()
    {
        if (_pageMoveAnimation != null) return;

        CurrentPage++;
        _pageMoveAnimation = StartCoroutine(MovePageCO());
    }

    /// <summary>
    /// Increments the current page index and starts the page move animation in the given direction
    /// </summary>
    /// <param name="direction">The direction of the movement</param>
    public void MovePage(int direction)
    {
        if (_pageMoveAnimation != null) return;

        CurrentPage += (int)Mathf.Sign(direction);
        _pageMoveAnimation = StartCoroutine(MovePageCO(direction));
    }

    /// <summary>
    /// Moves the quiz card in the given direction just outside of the viewport
    /// </summary>
    /// <param name="direction">The direction of the movement of the page. -1 is left and +1 is right. Default is right</param>
    private IEnumerator MovePageCO(int direction = 1)
    {
        OnMoveAnimationBeginAction?.Invoke();
        if (_moveAudioPlayer) _moveAudioPlayer.Play();

        var priorPos = transform.localPosition;
        var targetPos = priorPos + ((RectTransform)transform.parent).rect.width * direction * Vector3.left;

        float elapsedTime = 0f;

        while (elapsedTime < _animationDuration)
        {
            float t = elapsedTime / _animationDuration;

            var newPos = Vector3.Lerp(priorPos, targetPos, _pageMoveAnimationCurve.Evaluate(t));
            transform.localPosition = newPos;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = targetPos;

        OnMoveAnimationFinishedAction?.Invoke();

        _pageMoveAnimation = null;
    }
}
