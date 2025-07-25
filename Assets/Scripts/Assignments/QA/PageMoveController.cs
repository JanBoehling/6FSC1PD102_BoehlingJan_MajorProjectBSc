using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PageMoveController : MonoSingleton<PageMoveController>
{
    [SerializeField, Tooltip("The duration of the animation in seconds")] private float _animationDuration = 1f;
    [SerializeField] private AnimationCurve _pageMoveAnimationCurve;

    public Action OnMoveAnimationBeginAction { get; set; }
    public Action OnMoveAnimationFinishedAction { get; set; }

    public int CurrentPage { get; private set; }

    private Coroutine _pageMoveAnimation;

    public void MovePage()
    {
        if (_pageMoveAnimation != null) return;

        CurrentPage++;
        _pageMoveAnimation = StartCoroutine(MovePageCO());
    }

    public void MovePage(int direction)
    {
        if (_pageMoveAnimation != null) return;

        CurrentPage += (int)Mathf.Sign(direction);
        _pageMoveAnimation = StartCoroutine(MovePageCO(direction));
    }

    private IEnumerator MovePageCO(int direction = 1)
    {
        OnMoveAnimationBeginAction?.Invoke();

        var priorPos = transform.localPosition;
        var targetPos = priorPos + ((RectTransform)transform.parent).rect.width * direction * Vector3.left;

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

        if (!Application.isPlaying) return;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("<"))
        {
            PageMoveController.Instance.MovePage(-1);
        }

        else if (GUILayout.Button(">"))
        {
            PageMoveController.Instance.MovePage(1);
        }

        EditorGUILayout.EndHorizontal();
    }
    public override bool RequiresConstantRepaint() => true;
}
#endif
