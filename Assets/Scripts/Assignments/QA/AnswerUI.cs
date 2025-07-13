using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerUI : MonoBehaviour, IPointerClickHandler
{
    public bool IsCorrect { get; private set; }
    public bool IsSelected { get; private set; }
    public int Index { get; private set; }

    private TMP_Text _answerText;
    private ImageLoader _answerImage;
    private QuizCard _quizAssignmentController;
    private Image _backgroundImage;
    private Color _backgroundBaseColor;

    private void Awake()
    {
        _answerText = GetComponentInChildren<TMP_Text>();
        _answerImage = GetComponentInChildren<ImageLoader>();
        _backgroundImage = GetComponent<Image>();
        _backgroundBaseColor = _backgroundImage.color;
        _quizAssignmentController = transform.parent.GetComponentInParent<QuizCard>();
        if (!_quizAssignmentController) Debug.LogError($"{name}: Error: Could not fetch Quiz Assignment Controller.");
    }

    public void Init(string answerText, Sprite answerSprite, bool isCorrect, int index)
    {
        _answerText.text = answerText;
        _answerImage.LoadImage(answerSprite);
        IsCorrect = isCorrect;
        Index = index;
    }

    public void ToggleSelection()
    {
        IsSelected = !IsSelected;

        if (IsSelected)
        {
            var color = _backgroundImage.color;
            color.a = .8f;
            _backgroundImage.color = color;

            transform.localScale = Vector3.one * .8f;
        }
        else
        {
            _backgroundImage.color = _backgroundBaseColor;
            transform.localScale = Vector3.one;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleSelection();
    }
}
