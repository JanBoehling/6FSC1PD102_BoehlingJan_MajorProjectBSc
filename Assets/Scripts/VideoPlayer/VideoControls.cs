using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public abstract class VideoControls : MonoBehaviour
{
    [SerializeField] protected Image _progressBar;

    protected VideoPlayer _videoPlayer;

    protected float VideoProgress => (float)_videoPlayer.time / (float)_videoPlayer.length;

    protected virtual void Awake() => _videoPlayer = FindAnyObjectByType<VideoPlayer>();

    protected void Wind(Vector2 position)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_progressBar.rectTransform, position, null, out var localPoint))
        {
            float progress = Mathf.InverseLerp(_progressBar.rectTransform.rect.xMin, _progressBar.rectTransform.rect.xMax, localPoint.x);
            _videoPlayer.time = progress * _videoPlayer.length;
        }
    }
}
