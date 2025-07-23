using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VideoAssignmentController : MonoBehaviour
{
    public double CurrentWatchtime => _videoPlayer.time;
    public double VideoDuration => _videoPlayer.length;

    [SerializeField] private GameObject _videoLoadingScreen;
    [SerializeField] private RectTransform _quitMessageContainer;
    [SerializeField] private RectTransform _videoControlsContainer;
    [SerializeField] private Button _continueButton;
    [field: SerializeField] public VideoClip DebugVideoClip { get; private set; }
    [Space]
    [SerializeField] private float _timer;
    
    private VideoPlayer _videoPlayer;
    private AudioSource _audioPlayer;
    private ParticleSystem _confettiCanon;

    private Coroutine _timerCoroutine;

    private uint _assignmentID;

    private bool _hasLoadedOnce = false;

    private void Awake()
    {
        _videoPlayer = FindAnyObjectByType<VideoPlayer>();
        _audioPlayer = _videoPlayer.GetComponent<AudioSource>();

        _confettiCanon = FindFirstObjectByType<ParticleSystem>();
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _assignmentID = RuntimeDataHolder.CurrentMilestone.Assignments[0];
        var assignmentData = UnitAndAssignmentManager.Instance.GetAssignmentByID(_assignmentID) as VideoAssignment;

        StartVideo(assignmentData.VideoURL);

        if (!_hasLoadedOnce) _videoPlayer.prepareCompleted += PlayVideo;
        _hasLoadedOnce = true;
    }

    /// <summary>
    /// Debug version of Init that is used to directly load a specific video clip
    /// </summary>
    /// <param name="clip">The video clip that should be force played</param>
    public void Init(VideoClip clip)
    {
        _assignmentID = RuntimeDataHolder.CurrentMilestone.Assignments[0];
        var assignmentData = UnitAndAssignmentManager.Instance.GetAssignmentByID(_assignmentID) as VideoAssignment;

        StartVideo(clip);

        if (!_hasLoadedOnce) _videoPlayer.prepareCompleted += PlayVideo;
        _hasLoadedOnce = true;
    }

    /// <summary>
    /// Loads the video clip from the given URL and starts the video player
    /// </summary>
    /// <param name="videoURL">The URL of the video to be played</param>
    public void StartVideo(string videoURL)
    {
        _videoPlayer.EnableAudioTrack(0, true);
        _videoPlayer.SetTargetAudioSource(0, _audioPlayer);

        _videoPlayer.url = videoURL;

        _videoPlayer.Prepare();
    }

    /// <summary>
    /// Starts the video player with the given video clip
    /// </summary>
    /// <param name="videoClip">The video clip that should be played</param>
    public void StartVideo(VideoClip videoClip)
    {
        _videoPlayer.EnableAudioTrack(0, true);
        _videoPlayer.SetTargetAudioSource(0, _audioPlayer);

        _videoPlayer.clip = videoClip;

        _videoPlayer.Prepare();
    }

    /// <summary>
    /// Marks the milestone as completed, if the video has been watched to a given watch time threshold.
    /// </summary>
    public void FinishAssignment()
    {
        _videoPlayer.Pause();

        if (UnitAndAssignmentManager.Instance.GetAssignmentCompletionState(_assignmentID))
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
        _videoLoadingScreen.SetActive(false);

        src.Play();
        _audioPlayer.Play();

        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timer = 0f;
        }
        _timerCoroutine = StartCoroutine(VideoWatchtimeWatcherCO());
    }

    private IEnumerator VideoWatchtimeWatcherCO()
    {
        var assignmentData = UnitAndAssignmentManager.Instance.GetAssignmentByID(_assignmentID) as VideoAssignment;
        var minimumWatchtime = assignmentData.Duration * assignmentData.WatchtimePercentThreshold;

        while (true)
        {
            _timer += Time.deltaTime;

            if (_timer > minimumWatchtime)
            {
                UnitAndAssignmentManager.Instance.SetAssignmentCompletionState(_assignmentID);
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

        // Displays the current watchtime and the duration of the video
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox($"{_assignmentController.CurrentWatchtime:F2} / {_assignmentController.VideoDuration:F2}", MessageType.None, true);

        EditorGUILayout.Separator();

        if (GUILayout.Button("Force Play Video")) _assignmentController.Init(_assignmentController.DebugVideoClip);
    }

    public override bool RequiresConstantRepaint() => true;
}
#endif
