using UnityEngine;
using TMPro;

public class TopIslandController : MonoBehaviour
{
    /// <summary>
    /// If true, the title text is shown and the progress bars are hidden
    /// </summary>
    public bool IsDisplayingTitle
    {
        get => _isDisplayingTitle;
        set
        {
            _isDisplayingTitle = value;

            _titleTextContainer.SetActive(value);
            _progressDisplayContainer.SetActive(!value);
        }
    }

    [SerializeField] private GameObject _titleTextContainer;
    [SerializeField] private GameObject _progressDisplayContainer;
    
    private bool _isDisplayingTitle;

    /// <summary>
    /// Toggles between showing the title text and the progress bars
    /// </summary>
    public void ToggleTitle() => IsDisplayingTitle = !IsDisplayingTitle;

    /// <summary>
    /// Displays given text as title
    /// </summary>
    /// <param name="title">The text to be displayed as title</param>
    public void DisplayTitle(string title)
    {
        IsDisplayingTitle = true;
        _titleTextContainer.GetComponentInChildren<TMP_Text>().text = title;
    }
}
