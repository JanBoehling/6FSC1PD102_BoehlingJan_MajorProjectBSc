using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class SubtitleSystem : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TMP_Text _textField;

    [SerializeField] private GameObject _toggleSubtitlesButtonCrossout;
    private GameObject[] _uiElements;
    private bool _isEnabled = false;

    private List<Subtitle> _subtitles = new();

    [SerializeField] private int _currentIndex = -1;

    public void Init(VideoAssignment assignmentData) => _subtitles.AddRange(assignmentData.Subtitles);

    private void Awake()
    {
        _uiElements = new GameObject[transform.childCount];
        for (int i = 0; i < _uiElements.Length; i++)
        {
            _uiElements[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        if (!_isEnabled) return;

        if (!_videoPlayer.isPrepared) return;

        if (_subtitles.Count - 1 == _currentIndex || _videoPlayer.time < _subtitles[_currentIndex + 1].Time) return;
        
        _textField.text = _subtitles[++_currentIndex].Text;
    }

    public void ToggleSubtitleSystem()
    {
        _isEnabled = !_isEnabled;
        foreach (var uiElement in _uiElements)
        {
            uiElement.SetActive(_isEnabled);
        }
        _toggleSubtitlesButtonCrossout.SetActive(!_isEnabled);
    }

    public void RecalculateCurrentIndex(float percent)
    {
        _currentIndex = -1;
        return;
        for (int i = 0; i < _subtitles.Count; i++)
        {
            if (_videoPlayer.time < _subtitles[i].Time) continue;
            _currentIndex = i;
            _textField.text = _subtitles[_currentIndex].Text;
            return;
        }
    }
}
