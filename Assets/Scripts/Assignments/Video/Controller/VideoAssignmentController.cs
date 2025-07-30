using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoAssignmentController : MonoBehaviour
{
    public double CurrentWatchtime => _videoPlayer.time;
    public double VideoDuration => _videoPlayer.length;

    [SerializeField] private GameObject _videoLoadingScreen;
    [SerializeField] private RectTransform _quitMessageContainer;
    [SerializeField] private RectTransform _videoControlsContainer;
    [SerializeField] private Button _continueButton;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private float _onErrorRetryDelay = 2f;
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

    /// <summary>
    /// Gets the video information of the current video assignment and initiates the start of the video
    /// </summary>
    public void Init()
    {
        _assignmentID = RuntimeDataHolder.CurrentMilestone.Assignments[0];
        var assignmentData = UnitAndAssignmentManager.Instance.GetAssignmentByID(_assignmentID) as VideoAssignment;

        if (!_hasLoadedOnce) _videoPlayer.errorReceived += OnError;
        if (!_hasLoadedOnce) _videoPlayer.prepareCompleted += PlayVideo;
        _hasLoadedOnce = true;

        StartVideo(assignmentData.VideoURL);
    }

    /// <summary>
    /// Debug version of Init that is used to directly load a specific video clip
    /// </summary>
    /// <param name="clip">The video clip that should be force played</param>
    public void Init(VideoClip clip)
    {
        _assignmentID = RuntimeDataHolder.CurrentMilestone.Assignments[0];
        var assignmentData = UnitAndAssignmentManager.Instance.GetAssignmentByID(_assignmentID) as VideoAssignment;

        if (!_hasLoadedOnce) _videoPlayer.errorReceived += OnError;
        if (!_hasLoadedOnce) _videoPlayer.prepareCompleted += PlayVideo;
        _hasLoadedOnce = true;

        StartVideo(clip);
    }

    /// <summary>
    /// Retries the video initiation when an error occures
    /// </summary>
    /// <param name="source">The source video player</param>
    /// <param name="message">The message of the error</param>
    private void OnError(VideoPlayer source, string message)
    {
#if UNITY_EDITOR
        Debug.Log(source.name + ": " + message);
#endif
        _messageText.text = $"Das Video kann gerade nicht abgespielt werden...\nIch versuche es in {_onErrorRetryDelay:F2} Sekunden nochmal.";
        StartCoroutine(RetryCO());
    }

    /// <summary>
    /// Retries the video initiation after a predefined amount of seconds
    /// </summary>
    private IEnumerator RetryCO()
    {
        yield return new WaitForSeconds(_onErrorRetryDelay);

        _messageText.text = "Video lädt...";
        Init();
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

    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    public void ReturnToMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(1);

    /// <summary>
    /// Closes the quit message window, changes the on click action of the continue button and resumes video playback
    /// </summary>
    public void CloseQuitMessage()
    {
        _quitMessageContainer.gameObject.SetActive(false);
        _continueButton.onClick = new();
        _continueButton.onClick.AddListener(FinishAssignment);
        _videoPlayer.Play();
    }

    /// <summary>
    /// Starts the video and audio playback
    /// </summary>
    /// <param name="src">The video player that is supposed to play the video</param>
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

    /// <summary>
    /// Increments a timer in the background that tracks the watchtime
    /// </summary>
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
