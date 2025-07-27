using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayPauseController : VideoControlsBase, IPointerClickHandler
{
    [SerializeField] private Sprite _playIcon;
    [SerializeField] private Sprite _pauseIcon;
    [SerializeField] private Image _playPauseImage;
    [Space]
    [SerializeField] private float _animationDuration = 1f;

    private Coroutine _playPauseAnimation;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Pause();

            if (_playPauseAnimation != null) StopCoroutine(_playPauseAnimation);
            _playPauseAnimation = StartCoroutine(AnimatePlayPauseCO(_pauseIcon));
        }
        else
        {
            _videoPlayer.Play();

            if (_playPauseAnimation != null) StopCoroutine(_playPauseAnimation);
            _playPauseAnimation = StartCoroutine(AnimatePlayPauseCO(_playIcon));
        }
    }

    private IEnumerator AnimatePlayPauseCO(Sprite playPauseSprite)
    {
        _playPauseImage.sprite = playPauseSprite;

        _playPauseImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(_animationDuration);
        _playPauseImage.gameObject.SetActive(false);

        _playPauseAnimation = null;
    }
}
