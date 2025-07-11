using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VideoAssignmentController : MonoBehaviour
{
    public double CurrentWatchtime => _videoPlayer.time;
    public double VideoDuration => _videoPlayer.length;

    [SerializeField] private RawImage _videoPlayerImage;
    [SerializeField] private RectTransform _quitMessageContainer;
    [SerializeField] private RectTransform _videoControlsContainer;
    [SerializeField] private Button _continueButton;
    [Space]
    [SerializeField] private Texture _loadingVideoTexture;
    [SerializeField] private Texture _videoRenderTexture;
    [Space]
    [SerializeField] private VideoAssignment _debugVideoAssignment;
    [Space]
    [SerializeField] private float _timer;
    
    private VideoPlayer _videoPlayer;
    private AudioSource _audioPlayer;
    private ParticleSystem _confettiCanon;

    private bool _isFullscreen;

    private uint _assignmentID;

    private void Awake()
    {
        _videoPlayer = FindAnyObjectByType<VideoPlayer>();
        _audioPlayer = _videoPlayer.GetComponent<AudioSource>();

        _confettiCanon = FindFirstObjectByType<ParticleSystem>();
    }

    private void Start()
    {
        _assignmentID = RuntimeDataHolder.CurrentMilestone.Assignments[0];
        var assignmentData = CompletionTracker.Instance.GetAssignmentByID(_assignmentID) as VideoAssignment;

        if (assignmentData == null)
        {
            assignmentData = _debugVideoAssignment;
            Debug.LogWarning("Could not fetch video assignment data. Using debug video assignment instead.");
        }

        StartVideo(assignmentData.VideoURL);

        _videoPlayer.prepareCompleted += PlayVideo;
    }

    public void StartVideo(string videoURL)
    {
        _videoPlayer.EnableAudioTrack(0, true);
        _videoPlayer.SetTargetAudioSource(0, _audioPlayer);

        _videoPlayer.url = videoURL;
        _videoPlayerImage.texture = _loadingVideoTexture;

        _videoPlayer.Prepare();
    }

    /// <summary>
    /// Toggles fullscreen mode for video on tap on the video
    /// </summary>
    [Obsolete("This method is obsolete because in the future there will only be portrat videos")]
    public void ToggleFullscreen()
    {
        _isFullscreen = !_isFullscreen;

        // Rotates the orientation of the device
        Screen.orientation = _isFullscreen ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;

        // Switches from render texture to camera screen
        _videoPlayer.renderMode = _isFullscreen ? VideoRenderMode.CameraNearPlane : VideoRenderMode.RenderTexture;

        // Disables or enables the render texture view
        _videoPlayerImage.gameObject.SetActive(!_isFullscreen);

        // Moves the video control bar
        _videoControlsContainer.offsetMin = new Vector2(_isFullscreen ? 0f : 50f, _videoControlsContainer.offsetMin.y);
        _videoControlsContainer.offsetMax = new Vector2(_isFullscreen ? 0f : 50f, _videoControlsContainer.offsetMax.y);
        _videoControlsContainer.anchoredPosition = new Vector2(_videoControlsContainer.anchoredPosition.x, _isFullscreen ? 0f : 280f);
    }

    /// <summary>
    /// Marks the milestone as completed, if the video has been watched to a given watch time threshold.
    /// </summary>
    public void FinishAssignment()
    {
        _videoPlayer.Pause();

        if (CompletionTracker.Instance.GetAssignmentCompletionState(_assignmentID))
        {
            _confettiCanon.Play();
            _continueButton.image.color = Color.green;
            _continueButton.onClick = new();
            _continueButton.onClick.AddListener(ReturnToMenu);
        }
        else
        {
            _quitMessageContainer.gameObject.SetActive(true);
            _continueButton.onClick = new();
            _continueButton.onClick.AddListener(() => _quitMessageContainer.gameObject.SetActive(true));
        }
    }

    public void ReturnToMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(1);

    public void CloseQuitMessage()
    {
        _quitMessageContainer.gameObject.SetActive(false);
        _continueButton.onClick = new();
        _continueButton.onClick.AddListener(FinishAssignment);
        _videoPlayer.Play();
    }

    private void PlayVideo(VideoPlayer src)
    {
        _videoPlayerImage.texture = _videoRenderTexture;

        src.Play();
        _audioPlayer.Play();

        StartCoroutine(VideoWatchtimeWatcherCO());
    }

    private IEnumerator VideoWatchtimeWatcherCO()
    {
        var assignmentData = CompletionTracker.Instance.GetAssignmentByID(_assignmentID) as VideoAssignment;
        var minimumWatchtime = assignmentData.Duration * assignmentData.WatchtimePercentThreshold;

        while (true)
        {
            _timer += Time.deltaTime;

            if (_timer > minimumWatchtime)
            {
                CompletionTracker.Instance.SetAssignmentCompletionState(_assignmentID);
                break;
            }

            // Pauses the timer until the video continues
            if (_videoPlayer.isPaused) yield return new WaitWhile(() => _videoPlayer.isPaused);

            yield return null;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(VideoAssignmentController))]
public class VideoAssignmentControllerEditor : Editor
{
    private VideoAssignmentController _assignmentController;

    private void OnEnable()
    {
        _assignmentController = (VideoAssignmentController)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying) return;

        // Toggle fullscreen
        EditorGUILayout.Space();
        if (GUILayout.Button("Toggle Fullscreen")) _assignmentController.ToggleFullscreen();

        // Displays the current watchtime and the duration of the video
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox($"{_assignmentController.CurrentWatchtime:F2} / {_assignmentController.VideoDuration:F2}", MessageType.None, true);
    }

    public override bool RequiresConstantRepaint() => true;
}
#endif
