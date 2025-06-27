using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TopIslandController : MonoBehaviour
{
    [SerializeField] private GameObject _titleTextContainer;
    [SerializeField] private GameObject _progressDisplayContainer;
    
    private bool _isDisplayingTitle;

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

    /// <summary>
    /// Toggles between showing the title text and the progress bars
    /// </summary>
    public void ToggleTitle()
    {
        IsDisplayingTitle = !IsDisplayingTitle;
    }


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

#if UNITY_EDITOR
[CustomEditor(typeof(TopIslandController))]
public class TopIslandControllerEditor : Editor
{
    private TopIslandController _controller;

    private string _debugTitle;

    private void OnEnable()
    {
        _controller = (TopIslandController)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        if (GUILayout.Button("Toggle Title")) _controller.ToggleTitle();
        _debugTitle = EditorGUILayout.TextField(_debugTitle);
        if (GUILayout.Button("Display Title")) _controller.DisplayTitle(_debugTitle);
    }
}
#endif
