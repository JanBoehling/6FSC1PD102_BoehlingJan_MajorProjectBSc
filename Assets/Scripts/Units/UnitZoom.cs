using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Obsolete]
public class UnitZoom : MonoBehaviour
{
    [SerializeField] private float _zoomAnimationSpeed = 4f;
    [Space]
    [SerializeField] private Vector3 _zoomedPosition = new Vector3(0f, -2f, 0f);
    [SerializeField] private Vector3 _zoomedScale = new Vector3(4f, 4f, 4f);

    private readonly Vector3 _basePosition = Vector3.zero;
    private readonly Vector3 _baseScale = Vector3.one;

    private SineBounce _sineBounce;

    private Coroutine _currentZoomAnimation;

    private bool _isZoomedIn;

    private void Awake()
    {
        _sineBounce = GetComponent<SineBounce>();
    }

    public void ZoomIn()
    {
        if (_currentZoomAnimation != null) return;
        else if (_isZoomedIn) return;

        _currentZoomAnimation = StartCoroutine(ZoomInCO());
    }

    public void ZoomOut()
    {
        if (_currentZoomAnimation != null) return;
        else if (!_isZoomedIn) return;

        _currentZoomAnimation = StartCoroutine(ZoomOutCO());
    }

    public IEnumerator ZoomInCO()
    {
        if (_sineBounce) _sineBounce.enabled = false;
        transform.localPosition = _basePosition;

        float timer = 0f;

        while (true)
        {
            transform.localPosition = Vector3.Lerp(_basePosition, _zoomedPosition, timer);
            transform.localScale = Vector3.Lerp(_baseScale, _zoomedScale, timer);

            timer += Time.deltaTime * _zoomAnimationSpeed;
            if (timer >= 1f) break;

            yield return null;
        }

        transform.localPosition = _zoomedPosition;
        transform.localScale = _zoomedScale;

        _isZoomedIn = true;

        _currentZoomAnimation = null;
    }

    public IEnumerator ZoomOutCO()
    {
        float timer = 1f;

        while (true)
        {
            transform.localPosition = Vector3.Lerp(_basePosition, _zoomedPosition, timer);
            transform.localScale = Vector3.Lerp(_baseScale, _zoomedScale, timer);

            timer -= Time.deltaTime * _zoomAnimationSpeed;
            if (timer <= 0f) break;

            yield return null;
        }

        transform.localPosition = _basePosition;
        transform.localScale = _baseScale;

        if (_sineBounce) _sineBounce.enabled = true;

        _isZoomedIn = false;

        _currentZoomAnimation = null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UnitZoom))]
public class UnitObjectEditor : Editor
{
    private UnitZoom unitObject;

    private void OnEnable()
    {
        unitObject = (UnitZoom)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying) return;
        if (GUILayout.Button("Zoom in")) unitObject.ZoomIn();
        if (GUILayout.Button("Zoom out")) unitObject.ZoomOut();
    }
}
#endif
