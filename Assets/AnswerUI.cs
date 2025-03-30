using TMPro;
using UnityEngine;

public class AnswerUI : MonoBehaviour
{
    private TMP_Text _answerText;
    private SpriteToTextureConverter _answerImage;

    private void Awake()
    {
        _answerText = GetComponentInChildren<TMP_Text>();
        _answerImage = GetComponentInChildren<SpriteToTextureConverter>();
    }

    public void Init(string answerText, Sprite answerSprite)
    {
        _answerText.text = answerText;
        _answerImage.LoadImage(answerSprite);
    }
}
