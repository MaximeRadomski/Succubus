using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBhv : FrameRateBehavior
{
    private Vector3 _originalScale;
    private Vector3 _targetScale;
    private Vector3 _originalPosition;
    private Vector3 _targetPosition;

    enum BounceState
    {
        Bounce,
        Reset,
        Wait
    }

    private BounceState _state;
    private int _waitTimer;

    public void Init()
    {
        _state = BounceState.Wait;
        _waitTimer = 0;
        _originalScale = transform.localScale;
        _originalPosition = transform.localPosition;
        _targetScale = new Vector3(1.05f, 1.05f, 1.0f);
        _targetPosition = transform.localPosition + new Vector3(0.0f, Constants.Pixel * 4, 0.0f);
    }

    protected override void FrameUpdate()
    {
        if (_state == BounceState.Wait)
        {
            if (_waitTimer < 30)
                ++_waitTimer;
            else
            {
                _waitTimer = 0;
                _state = BounceState.Bounce;
            }
        }
        else if (_state == BounceState.Bounce)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, 0.15f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, 0.15f);

            if (Helper.VectorEqualsPrecision(transform.localPosition, _targetPosition, 0.01f))
            {
                transform.localScale = _targetScale;
                transform.localPosition = _targetPosition;
                _state = BounceState.Reset;
            }
        }
        else if (_state == BounceState.Reset)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.15f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, _originalPosition, 0.15f);

            if (Helper.VectorEqualsPrecision(transform.localPosition, _originalPosition, 0.01f))
            {
                transform.localScale = _originalScale;
                transform.localPosition = _originalPosition;
                _state = BounceState.Wait;
            }
        }
    }
}
