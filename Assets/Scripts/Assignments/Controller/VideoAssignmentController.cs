using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoAssignmentController : MonoBehaviour
{
    public VideoAssignment AssignmentData { get; private set; }

    [SerializeField] private RawImage _videoPlayerImage;
    [Space]
    [SerializeField] private Texture _loadingVideoTexture;
    [SerializeField] private Texture _videoRenderTexture;
    
    private VideoPlayer _videoPlayer;
    private float _timer;

    private void Awake()
    {
        _videoPlayer = FindAnyObjectByType<VideoPlayer>();
    }

    public void Init(VideoAssignment video)
    {
        _videoPlayer.clip = AssignmentData.Video;

        _videoPlayer.prepareCompleted += PlayVideo;

        AssignmentData = video;
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
        StartCoroutine(VideoWatchtimeWatcherCO());
    }

    private IEnumerator VideoWatchtimeWatcherCO()
    {
        var minimumWatchtime = AssignmentData.Duration * AssignmentData.WatchtimePercentThreshold;

        while (true)
        {
            _timer += Time.deltaTime;

            if (_timer > minimumWatchtime)
            {
                AssignmentData.IsCompleted = true;
                break;
            }

            yield return null;
        }
    }
}
