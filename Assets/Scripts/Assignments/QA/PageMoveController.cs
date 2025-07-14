using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PageMoveController : MonoSingleton<PageMoveController>
{
    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private AnimationCurve _pageMoveAnimationCurve;

    public Action OnMoveAnimationBeginAction { get; set; }
    public Action OnMoveAnimationFinishedAction { get; set; }

    private int _pageAmount = 0;

    private Button[] _buttons;

    public int CurrentPage { get; private set; }

    private Coroutine _pageMoveAnimation;

    protected override void Awake()
    {
        base.Awake();
        _buttons = GetComponentsInChildren<Button>();
    }

    public void MovePage()
    {
        if (_pageMoveAnimation != null) return;

        CurrentPage++;
        _pageMoveAnimation = StartCoroutine(MovePageCO());
    }

    public void SetPageAmount(int amount)
    {
        _pageAmount = amount;
    }

    private IEnumerator MovePageCO()
    {
        OnMoveAnimationBeginAction?.Invoke();

        var priorPos = transform.localPosition;
        var targetPos = priorPos + ((RectTransform)transform.parent).rect.width * Vector3.left;

        float elapsedTime = 0f;

        while (elapsedTime < _animationDuration)
        {
            float t = elapsedTime / _animationDuration;

            var newPos = Vector3.Lerp(priorPos, targetPos, _pageMoveAnimationCurve.Evaluate(t));
            transform.localPosition = newPos;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = targetPos;

        OnMoveAnimationFinishedAction?.Invoke();

        _pageMoveAnimation = null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PageMoveController))]
public class PageMoverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField($"Current Page: {PageMoveController.Instance.CurrentPage}");
    }
    public override bool RequiresConstantRepaint() => true;
}
#endif
