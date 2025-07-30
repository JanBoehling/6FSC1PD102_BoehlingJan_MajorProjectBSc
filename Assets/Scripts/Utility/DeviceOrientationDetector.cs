using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DeviceOrientationDetector : MonoBehaviour
{
    public (float width, float height) ScreenSizeInWorldSpace { get; private set; }
    public (int width, int height) ScreenSize { get; private set; }

    [SerializeField] private bool _checkWidth = true;
    [SerializeField] private bool _checkHeight = false;
    [Space]
    [SerializeField] private UnityEvent _onDeviceOrientationChangedEvent;

    private static readonly Func<int> _widthComparator = () => Screen.width;
    private static readonly Func<int> _heightComparator = () => Screen.height;

    private Camera _cam;

    private void Awake() => _cam = Camera.main;

    private void Start()
    {
        ScreenSizeInWorldSpace = ScreenWidthToWorldSpace();
        ScreenSize = RecalculateScreenSize();
    }

    private void LateUpdate()
    {
        if ((!_checkWidth || ScreenSize.width == _widthComparator.Invoke()) && (!_checkHeight || ScreenSize.height != _heightComparator.Invoke())) return;

        ScreenSizeInWorldSpace = ScreenWidthToWorldSpace();
        ScreenSize = RecalculateScreenSize();
    }

    /// <summary>
    /// Sets the current screen width and height and invokes orientation changed event
    /// </summary>
    /// <returns>The width and height of the screen</returns>
    public (int width, int height) RecalculateScreenSize()
    {
        var screenSize = (width: default(int), height: default(int));

        if (_checkWidth) screenSize.width = _widthComparator.Invoke();
        if (_checkHeight) screenSize.height = _heightComparator.Invoke();

        if (_onDeviceOrientationChangedEvent != null) StartCoroutine(DelayedExecution(_onDeviceOrientationChangedEvent.Invoke));

        return screenSize;
    }

    /// <summary>
    /// Convertes the screen size to world coordinates
    /// </summary>
    /// <returns>The width and height of the screen in world space</returns>
    public (float width, float height) ScreenWidthToWorldSpace()
    {
        var screenSize = (width: default(float), height: default(float));

        screenSize.width = _cam.orthographicSize * _cam.aspect;
        screenSize.height = _cam.orthographicSize * 2f;

        return screenSize;
    }

    /// <summary>
    /// Waits to the next frame to execute given action
    /// </summary>
    /// <param name="delayedAction">The action that should be executed next frame</param>
    private IEnumerator DelayedExecution(Action delayedAction)
    {
        yield return null;
        delayedAction?.Invoke();
    }
}
