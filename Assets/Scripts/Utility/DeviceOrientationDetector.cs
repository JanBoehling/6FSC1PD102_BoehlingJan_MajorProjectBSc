using System;
using UnityEngine;
using UnityEngine.Events;

public class DeviceOrientationDetector : MonoBehaviour
{
    [SerializeField] private bool _checkWidth = true;
    [SerializeField] private bool _checkHeight = false;
    [Space]
    [SerializeField] private UnityEvent _onDeviceOrientationChangedEvent;

    //private static readonly Func<float> _widthComparator = () => Screen.width;
    //private static readonly Func<float> _heightComparator = () => Screen.height;

    public static float Width { get; private set; }
    public static float Height { get; private set; }

    private void Start()
    {
        var screenSize = GetScreenSize();

        Width = screenSize.x;
        Height = screenSize.y;
    }

    private void LateUpdate()
    {
        var screenSize = GetScreenSize();

        if ((!_checkWidth || Width == screenSize.x) && (!_checkHeight || Height != screenSize.y)) return;

        if (_checkWidth) Width = screenSize.x;
        if (_checkHeight) Height = screenSize.y;

        _onDeviceOrientationChangedEvent?.Invoke();
    }

    public Vector2 GetScreenSize()
    {
        var screenSize = new Vector2(Screen.width, Screen.height);
        var screenSizeWorld = Camera.main.ScreenToWorldPoint(screenSize);
        return screenSizeWorld;
    }
}
