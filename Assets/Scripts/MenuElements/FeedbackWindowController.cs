using UnityEngine;
using System.Runtime.InteropServices;

public class FeedbackWindowController : MonoBehaviour
{
    [SerializeField] private string _feedbackFormURL;

    /// <summary>
    /// Opens the given link. When in build, uses the JavaScript backend to open the link in a new tab
    /// </summary>
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
