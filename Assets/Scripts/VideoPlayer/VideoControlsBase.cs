using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public abstract class VideoControlsBase : MonoBehaviour
{
    [SerializeField] protected Image _progressBar;

    protected VideoPlayer _videoPlayer;

    protected float VideoProgress => (float)_videoPlayer.time / (float)_videoPlayer.length;

    protected virtual void Awake() => _videoPlayer = FindAnyObjectByType<VideoPlayer>();

    /// <summary>
    /// Winds the video back or forth depending on the given positional offset on the bar
    /// </summary>
    /// <param name="position">The positional offset on the bar</param>
    protected void Wind(Vector2 position)
    {
        if (!_progressBar)
        {
            Debug.LogError($"{name}: No progress bar selected.");
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_progressBar.rectTransform, position, null, out var localPoint)) return;

        float progress = Mathf.InverseLerp(_progressBar.rectTransform.rect.xMin, _progressBar.rectTransform.rect.xMax, localPoint.x);
        _videoPlayer.time = progress * _videoPlayer.length;
    }
}
