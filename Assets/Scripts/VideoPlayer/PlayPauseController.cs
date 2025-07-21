using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayPauseController : VideoControlsBase, IPointerClickHandler
{
    [SerializeField] private UnityEvent _onPlayEvent;
    [SerializeField] private UnityEvent _onPauseEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Pause();
            _onPauseEvent?.Invoke();
        }
        else
        {
            _videoPlayer.Play();
            _onPlayEvent?.Invoke();
        }
    }
}
