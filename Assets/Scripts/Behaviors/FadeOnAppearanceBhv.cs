using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnAppearanceBhv : FrameRateBehavior
{
    public bool SelfInit;
    public float Speed;
    public Color FadeColor;

    private SpriteRenderer _renderer;

    private bool _isFading;

    private void Start()
    {
        if (SelfInit)
            Init(Speed, FadeColor);
    }

    public void Init(float speed, Color? color)
    {
        _renderer = gameObject.GetComponent<SpriteRenderer>();
        if (color != null)
            _renderer.color = color.Value;
        FadeColor = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0.0f);
        Speed = speed;
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
        _renderer.color = Color.Lerp(_renderer.color, FadeColor, Speed);
        if (Helper.FloatEqualsPrecision(_renderer.color.a, FadeColor.a, 0.005f))
        {
            _isFading = false;
            _renderer.color = FadeColor;
            if (gameObject != null)
                Destroy(gameObject);
        }
    }
}
