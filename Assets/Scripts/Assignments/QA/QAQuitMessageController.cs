using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QAQuitMessageController : MonoBehaviour
{
    public Button CloseMessageButton => _closeMessageButton;
    public Button ConfirmAbortButton => _confirmAbortButton;

    [SerializeField] private Button _closeMessageButton;
    [SerializeField] private Button _confirmAbortButton;
    [SerializeField] private TMP_Text _abortMessageTextDisplay;
    [Space]
    [SerializeField, TextArea] private string _defaultAbortMessage;
    [SerializeField, TextArea] private string _quitOnAssignmentFailureMessage;

    private QAAbortMessage _messageOnEnable;

    private void OnEnable() => _abortMessageTextDisplay.text = _messageOnEnable switch
    {
        QAAbortMessage.Abort => _defaultAbortMessage,
        QAAbortMessage.OnAssignmentFailiure => _quitOnAssignmentFailureMessage,
        _ => _abortMessageTextDisplay.text
    };

    /// <summary>
    /// Defines what type of message is displayed when quit message window is activated
    /// </summary>
    /// <param name="message">The type of message to be displayed</param>
    public void SelectMessageOnEnable(QAAbortMessage message) => _messageOnEnable = message;

    /// <summary>
    /// Displays a specific text in the quit message window
    /// </summary>
    /// <param name="text">The text that should be displayed</param>
    public void DisplayText(string text) => _abortMessageTextDisplay.text = text;
}

public enum QAAbortMessage
{
    None,
    Abort,
    OnAssignmentFailiure
}
