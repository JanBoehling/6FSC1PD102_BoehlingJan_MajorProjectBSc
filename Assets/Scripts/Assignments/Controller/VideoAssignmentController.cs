using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VideoAssignmentController : MonoBehaviour
{
    public VideoAssignment AssignmentData { get; private set; }

    public double CurrentWatchtime => _videoPlayer.time;
    public double VideoDuration => _videoPlayer.length;

    [SerializeField] private RawImage _videoPlayerImage;
    [SerializeField] private Image _backgroundImage;
    [Space]
    [SerializeField] private Texture _loadingVideoTexture;
    [SerializeField] private Texture _videoRenderTexture;
    [Space]
    [SerializeField] private VideoAssignment _debugVideoAssignment;
    
    private VideoPlayer _videoPlayer;
    private float _timer;

    private bool _isFullscreen;

    private void Awake()
    {
        _videoPlayer = FindAnyObjectByType<VideoPlayer>();
    }

    private void Start()
    {
        var currentMilestone = RuntimeDataHolder.CurrentMilestone;
        var videoAssignment = currentMilestone?.Assignments[0] as VideoAssignment;

        if (videoAssignment != null) AssignmentData = videoAssignment;
        else
        {
            AssignmentData = _debugVideoAssignment;
            Debug.LogWarning("Could not fetch video assignment data. Using debug video assignment instead.");
        }

        _videoPlayer.clip = AssignmentData.Video;

        _videoPlayer.prepareCompleted += PlayVideo;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.LogWarning("A");
            
        }
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
        _isFullscreen = !_isFullscreen;

        Screen.orientation = ScreenOrientation.LandscapeLeft;
        _videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;

        _videoPlayerImage.gameObject.SetActive(false);
        _backgroundImage.gameObject.SetActive(false);
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
