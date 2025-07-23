using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FeedbackWindowController : MonoBehaviour
{
    [SerializeField] private string _feedbackFormURL;

    private Image _image;
    private GameObject _container;

    public void Awake()
    {
        _image = GetComponent<Image>();
        _container = transform.GetChild(0).gameObject;
    }

    public void ToggleVisibility()
    {
        _image.enabled = !_image.enabled;
        _container.SetActive(!_container.activeSelf);
    }

    public void OpenFeedbackForm()
    {
#if !UNITY_EDITOR
		OpenWindow(_feedbackFormURL);
#endif
    }

    [DllImport("__Internal")]
    private static extern void OpenWindow(string url);
}

#if UNITY_EDITOR
[CustomEditor(typeof(FeedbackWindowController))]
public class FeedbackWindowControllerEditor : Editor
{
    private FeedbackWindowController _controller;

    private void OnEnable()
    {
        _controller = (FeedbackWindowController)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!GUILayout.Button("Toggle Visibility")) return;

        _controller.Awake();
        _controller.ToggleVisibility();
    }
}
#endif
