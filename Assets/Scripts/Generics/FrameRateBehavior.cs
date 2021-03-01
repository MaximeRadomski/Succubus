using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FrameRateBehavior : MonoBehaviour
{
    private int _framesPerSecond = Constants.MaxFps;
    private float? _frameValue = null;
    private float? _lastFrameValue = null;
    protected float _frame
    {
        get
        {
            if (_frameValue == null)
                SetFrameValue();
            return _frameValue.Value;
        }
    }
    protected float _lastFrame
    {
        get
        {
            if (_lastFrameValue == null)
                SetLastFrameValue();
            return _lastFrameValue.Value;
        }
        set
        {
            _lastFrameValue = value;
        }
    }

    private void SetFrameValue()
    {
        var twentieth = (float)Constants.MaxFps / 20.0f;
        if (Screen.currentResolution.refreshRate >= Constants.MaxFps + twentieth)
            _frameValue = 1.0f / (_framesPerSecond + twentieth);
        else
            _frameValue = 0.0f;
    }

    private void SetLastFrameValue()
    {
        _lastFrame = Time.time - _frame;
    }

    void Update()
    {
        if (Time.time < _lastFrame + _frame)
            return;
        _lastFrame = Time.time;
        FrameUpdate();
    }

    protected virtual void FrameUpdate()
    {

    }
}
