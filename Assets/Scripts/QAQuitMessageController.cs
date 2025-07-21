using TMPro;
using UnityEngine;

public class QAQuitMessageController : MonoBehaviour
{
    [SerializeField] private TMP_Text _abortMessageTextDisplay;
    [Space]
    [SerializeField, TextArea] private string _defaultAbortMessage;
    [SerializeField, TextArea] private string _quitOnAssignmentFailureMessage;

    private QAAbortMessage _messageOnEnable;

    private void OnEnable() => _abortMessageTextDisplay.text = _messageOnEnable switch
    {
        QAAbortMessage.Abort => _defaultAbortMessage,
        QAAbortMessage.OnAssignmentFailiure => _quitOnAssignmentFailureMessage,
        _ => ""
    };

    public void SelectMessageOnEnable(QAAbortMessage message) => _messageOnEnable = message;
}

public enum QAAbortMessage
{
    Abort,
    OnAssignmentFailiure
}
