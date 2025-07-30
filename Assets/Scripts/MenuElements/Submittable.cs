using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Gives thes user the ability to submit a text field input
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class Submittable : MonoBehaviour
{
    [field: SerializeField] public UnityEvent<string> OnSubmit { get; private set; } = new();

    private TMP_InputField _inputField;

    private void Awake() => _inputField = GetComponent<TMP_InputField>();

    private void Start() => _inputField.onSubmit.AddListener(Submit);

    private void OnDestroy() => _inputField.onSubmit.RemoveListener(Submit);

    /// <summary>
    /// Submits the given string
    /// </summary>
    public void Submit(string value) => OnSubmit?.Invoke(value);
    
    /// <summary>
    /// Submits the value from the input field
    /// </summary>
    public void Submit() => OnSubmit?.Invoke(_inputField.text);
}
