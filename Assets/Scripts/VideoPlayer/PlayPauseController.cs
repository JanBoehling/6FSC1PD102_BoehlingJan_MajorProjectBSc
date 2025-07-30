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
        // Plays or pauses the videos and briefly shows a sprite inducating the play or pause state
        if (VideoPlayer.isPlaying)
        {
            VideoPlayer.Pause();

            if (_playPauseAnimation != null) StopCoroutine(_playPauseAnimation);
            _playPauseAnimation = StartCoroutine(AnimatePlayPauseCO(_pauseIcon));
        }
        else
        {
            VideoPlayer.Play();

            if (_playPauseAnimation != null) StopCoroutine(_playPauseAnimation);
            _playPauseAnimation = StartCoroutine(AnimatePlayPauseCO(_playIcon));
        }
    }

    /// <summary>
    /// Shows the given sprite for a predefined amount of seconds and afterwards hides it again
    /// </summary>
    /// <param name="playPauseSprite">The sprite that should be displayed</param>
    private IEnumerator AnimatePlayPauseCO(Sprite playPauseSprite)
    {
        _playPauseImage.sprite = playPauseSprite;

        _playPauseImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(_animationDuration);
        _playPauseImage.gameObject.SetActive(false);

        _playPauseAnimation = null;
    }
}
