using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoAssignmentController : MonoBehaviour
{
    private VideoPlayer _videoPlayer;
    private RawImage _videoPlayerImage;
    [Space]
    private Texture _loadingVideoTexture;
    private Texture _videoRenderTexture;


    private void Awake()
    {
        _videoPlayer = FindAnyObjectByType<VideoPlayer>();
    }

    private void Start()
    {
        _videoPlayer.prepareCompleted += PlayVideo;
    }

    public void StartVideo()
    {
        _videoPlayerImage.texture = _loadingVideoTexture;
        _videoPlayer.Prepare();
    }

    /// <summary>
    /// Toggles fullscreen mode for video on tap on the video
    /// </summary>
    public void ToggleFullscreen()
    {

    }

    /// <summary>
    /// Marks the milestone as completed, if the video has been watched to a given watch time threshold.
    /// </summary>
    public void FinishAssignment()
    {

    }

    private void PlayVideo(VideoPlayer src)
    {
        _videoPlayerImage.texture = _videoRenderTexture;
        src.Play();
    }
}
