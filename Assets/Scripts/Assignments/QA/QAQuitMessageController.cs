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

    public void SelectMessageOnEnable(QAAbortMessage message) => _messageOnEnable = message;

    public void DisplayText(string text) => _abortMessageTextDisplay.text = text;
}

public enum QAAbortMessage
{
    None,
    Abort,
    OnAssignmentFailiure
}
