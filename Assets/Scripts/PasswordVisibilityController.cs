using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordVisibilityController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    
    [SerializeField] private Sprite _hiddenSprite;
    [SerializeField] private Sprite _visibleSprite;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void TogglePasswordVisibility()
    {
        if (_inputField.contentType == TMP_InputField.ContentType.Password)
        {
            _inputField.contentType = TMP_InputField.ContentType.Standard;
            if (_visibleSprite) _image.sprite = _visibleSprite;
        }
        else
        {
            _inputField.contentType = TMP_InputField.ContentType.Password;
            if (_hiddenSprite) _image.sprite = _hiddenSprite;
        }

        _inputField.ForceLabelUpdate();
    }
}
