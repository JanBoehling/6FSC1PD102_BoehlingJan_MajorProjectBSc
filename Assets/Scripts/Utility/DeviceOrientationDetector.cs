using System;
using UnityEngine;
using UnityEngine.Events;

public class DeviceOrientationDetector : MonoBehaviour
{
    [SerializeField] private bool _checkWidth = true;
    [SerializeField] private bool _checkHeight = false;
    [Space]
    [SerializeField] private UnityEvent _onDeviceOrientationChangedEvent;

    private static readonly Func<int> _widthComparator = () => Screen.width;
    private static readonly Func<int> _heightComparator = () => Screen.height;

    private int _width;
    private int _height;

    private void Start()
    {
        _width = Screen.width;
        _height = Screen.height;
    }

    private void Update()
    {
        if ((!_checkWidth || _width == _widthComparator.Invoke()) && (!_checkHeight || _height != _heightComparator.Invoke())) return;

        if (_checkWidth) _width = _widthComparator.Invoke();
        if (_checkHeight) _height = _heightComparator.Invoke();

        _onDeviceOrientationChangedEvent?.Invoke();
    }
}
