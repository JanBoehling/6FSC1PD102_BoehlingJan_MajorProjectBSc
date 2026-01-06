using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public abstract class VideoControlsBase : MonoBehaviour
{
    [SerializeField] protected Image ProgressBar;
    [Space]
    [SerializeField] private UnityEvent<float> OnWind;

    protected VideoPlayer VideoPlayer;

    protected float VideoProgress => (float)VideoPlayer.time / (float)VideoPlayer.length;

    protected virtual void Awake() => VideoPlayer = FindAnyObjectByType<VideoPlayer>();

    /// <summary>
    /// Winds the video back or forth depending on the given positional offset on the bar
    /// </summary>
    /// <param name="position">The positional offset on the bar</param>
    protected void Wind(Vector2 position)
    {
        if (!ProgressBar)
        {
#if UNITY_EDITOR
            Debug.LogError($"{name}: No progress bar selected.");
#endif
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ProgressBar.rectTransform, position, null, out var localPoint)) return;

        float progress = Mathf.InverseLerp(ProgressBar.rectTransform.rect.xMin, ProgressBar.rectTransform.rect.xMax, localPoint.x);
        VideoPlayer.time = progress * VideoPlayer.length;

        OnWind?.Invoke(progress);
    }
}
