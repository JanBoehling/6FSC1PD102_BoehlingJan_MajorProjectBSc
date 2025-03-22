using UnityEngine;

public class SineBounce : MonoBehaviour
{
    [Tooltip("The speed in which the game object moves from min to max")]
    [SerializeField] private float _bounceSpeed = 2f;
    
    [Tooltip("The range of motion from the original position of the game object")]
    [SerializeField] private Vector3 _offset = new Vector3(0, .05f, 0);

    private Vector3 _basePosition;

    private void Start()
    {
        _basePosition = transform.localPosition;
    }

    private void Update()
    {
        LerpPosition(_basePosition - _offset, _basePosition + _offset);
    }

    /// <summary>
    /// Lerps the position of the game object between min and max in a sine movement
    /// </summary>
    /// <param name="min">The lowest, or left-most position</param>
    /// <param name="max">The greatest, or right-most position</param>
    private void LerpPosition(Vector3 min, Vector3 max)
    {
        float percent = Mathf.PingPong(Time.time * _bounceSpeed, 1);
        transform.localPosition = Vector3.Lerp(min, max, percent);
    }
}
