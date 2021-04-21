using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiggleBhv : FrameRateBehavior
{
    public bool IsOn;

    private Vector3 _originalScale;
    private Vector3? _targetScale;
    private float _rotationStrength;
    private int _maxRotationTicks;

    private bool _isResetingScale;
    private bool _isResetingRotation;
    private int _rotationTicks;
    private float _hideTimer;

    private SpriteRenderer _spriteRenderer;
    private System.Func<bool> _onHide;

    void Start()
    {
        _originalScale = transform.localScale;
        _rotationStrength = 1f;
        _maxRotationTicks = 30;
        _rotationTicks = 0;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Hide();
    }

    public void AppearAndWiggle(int seconds, System.Func<bool> onHide)
    {
        _onHide = onHide;
        _hideTimer = Time.time + (float)seconds;
        Appear();
        Wiggle();
    }

    public void Appear()
    {
        _spriteRenderer.color = Constants.ColorPlain;
    }

    public void Wiggle()
    {
        IsOn = true;
    }

    public void Hide()
    {
        IsOn = false;
        _spriteRenderer.color = Constants.ColorPlainTransparent;
    }

    protected override void FrameUpdate()
    {
        if (IsOn)
        {
            Wiggling();
            if (Time.time >= _hideTimer)
                Hide();
        }
    }

    private void Wiggling()
    {
        //SCALE
        if (_targetScale == null)
            _targetScale = new Vector3(1.3f, 1.3f, 1.0f);

        if (_isResetingScale)
            transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.1f);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale.Value, 0.1f);

        if ((_isResetingScale && Helper.VectorEqualsPrecision(transform.localScale, _originalScale, 0.01f))
            || (!_isResetingScale && Helper.VectorEqualsPrecision(transform.localScale, _targetScale.Value, 0.01f)))
        {
            if (_isResetingScale)
            {
                transform.localScale = _originalScale;
                _targetScale = null;
            }
            _isResetingScale = !_isResetingScale;
        }

        //ROTATION
        transform.Rotate(0.0f, 0.0f, _rotationStrength);
        if (++_rotationTicks >= _maxRotationTicks)
        {
            _rotationTicks = 0;
            _rotationStrength = -_rotationStrength;
        }
    }
}
