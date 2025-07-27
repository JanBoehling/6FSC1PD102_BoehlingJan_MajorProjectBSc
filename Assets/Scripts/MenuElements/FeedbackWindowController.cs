using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;

public class FeedbackWindowController : MonoBehaviour
{
    [SerializeField] private string _feedbackFormURL;
    [Space]
    [SerializeField] private UnityEvent _onEnable;
    [SerializeField] private UnityEvent _onDisable;

    private void OnEnable() => _onEnable?.Invoke();

    private void OnDisable() => _onDisable?.Invoke();

    public void OpenFeedbackForm()
    {
#if !UNITY_EDITOR
		OpenWindow(_feedbackFormURL);
#else
        Application.OpenURL(_feedbackFormURL);
#endif
    }

    [DllImport("__Internal")]
    private static extern void OpenWindow(string url);
}
