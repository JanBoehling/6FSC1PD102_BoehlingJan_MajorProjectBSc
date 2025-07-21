using TMPro;
using UnityEngine;

public class PasswordVisibilityController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    
    [SerializeField] private Sprite _hiddenSprite;
    [SerializeField] private Sprite _visibleSprite;

    public void TogglePasswordVisibility()
    {
        if (_inputField.contentType == TMP_InputField.ContentType.Password)
        {
            _inputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            _inputField.contentType = TMP_InputField.ContentType.Password;
        }

        _inputField.ForceLabelUpdate();
    }
}
