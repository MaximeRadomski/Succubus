using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnAppearanceBhv : FrameRateBehavior
{
    private SpriteRenderer _renderer;

    private bool _isFading;
    private float _speed;

    private Color _fadeColor;

    public void Init(float speed, Color? color)
    {
        _renderer = gameObject.GetComponent<SpriteRenderer>();
        if (color != null)
            _renderer.color = color.Value;
        _fadeColor = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0.0f);
        _speed = speed;
        Fade();
    }

    public void Fade()
    {
        _isFading = true;
    }

    protected override void FrameUpdate()
    {
        if (_isFading)
            Fading();
    }

    private void Fading()
    {
        _renderer.color = Color.Lerp(_renderer.color, _fadeColor, _speed);
        if (Helper.FloatEqualsPrecision(_renderer.color.a, _fadeColor.a, 0.005f))
        {
            _isFading = false;
            _renderer.color = _fadeColor;
            Destroy(gameObject);
        }
    }
}
