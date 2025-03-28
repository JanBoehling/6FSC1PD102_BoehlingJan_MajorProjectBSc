using TMPro;
using UnityEngine;

public class AnswerUI : MonoBehaviour
{
    private TMP_Text _answerText;
    private QuestionImageLoader _answerImage;

    private void Start()
    {
        _answerText = GetComponentInChildren<TMP_Text>();
        _answerImage = GetComponentInChildren<QuestionImageLoader>();
    }

    public void Init(string answerText, Sprite answerSprite)
    {
        _answerText.text = answerText;
        _answerImage.LoadImage(answerSprite);
    }
}
