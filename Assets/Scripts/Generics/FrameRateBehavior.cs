using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FrameRateBehavior : MonoBehaviour
{
    private int _framesPerSecond = 60;
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
        _frameValue = 1.0f / (_framesPerSecond + 1);        
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

    protected abstract void FrameUpdate();
}
