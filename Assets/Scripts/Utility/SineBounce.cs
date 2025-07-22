using UnityEngine;

public class SineBounce : MonoBehaviour
{
    [Tooltip("The speed in which the game object moves from min to max")]
    [SerializeField] private float _bounceSpeed = 2f;
    
    [Tooltip("The range of motion from the original position of the game object")]
    [SerializeField] private Vector3 _offset = new Vector3(0, .05f, 0);

    private Transform _transform;

    private Vector3 _basePosition;

    private void Awake()
    {
        _transform = TryGetComponent<RectTransform>(out var rectTransform) ? rectTransform : transform;
    }

    private void Update() => LerpPosition(_basePosition - _offset, _basePosition + _offset);

    public void Init() => _basePosition = _transform is RectTransform ? (_transform as RectTransform).offsetMax : transform.localPosition;

    /// <summary>
    /// Lerps the position of the game object between min and max in a sine movement
    /// </summary>
    /// <param name="min">The lowest, or left-most position</param>
    /// <param name="max">The greatest, or right-most position</param>
    private void LerpPosition(Vector3 min, Vector3 max)
    {
        float percent = Mathf.PingPong(Time.time * _bounceSpeed, 1);

        if (_transform is RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(min, max, percent);
        }
        else transform.localPosition = Vector3.Lerp(min, max, percent);
    }
}
