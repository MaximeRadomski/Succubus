using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnAppearanceBhv : FrameRateBehavior
{
    public bool SelfInit;
    public float Speed;
    public Color FadeColor;

    private SpriteRenderer _renderer;
    private TMPro.TextMeshPro _textRenderer;

    private bool _isFading;

    private void Start()
    {
        if (SelfInit)
            Init(Speed, FadeColor);
    }

    public void Init(float speed, Color? color)
    {
        _renderer = gameObject.GetComponent<SpriteRenderer>();
        _textRenderer = gameObject.GetComponent<TMPro.TextMeshPro>();
        if (color != null && _renderer != null)
            _renderer.color = color.Value;
        if (color != null && _textRenderer != null)
            _textRenderer.color = color.Value;
        if (_renderer != null)
            FadeColor = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0.0f);
        else if (_textRenderer != null)
            FadeColor = new Color(_textRenderer.color.r, _textRenderer.color.g, _textRenderer.color.b, 0.0f);
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
        if (_renderer != null)
            _renderer.color = Color.Lerp(_renderer.color, FadeColor, Speed);
        if (_textRenderer != null)
            _textRenderer.color = Color.Lerp(_textRenderer.color, FadeColor, Speed);
        if (_renderer != null && Helper.FloatEqualsPrecision(_renderer.color.a, FadeColor.a, 0.005f))
        {
            _isFading = false;
            _renderer.color = FadeColor;
            if (gameObject != null)
                Destroy(gameObject);
        }
        if (_textRenderer != null && Helper.FloatEqualsPrecision(_textRenderer.color.a, FadeColor.a, 0.005f))
        {
            _isFading = false;
            _textRenderer.color = FadeColor;
            if (gameObject != null)
                Destroy(gameObject);
        }
    }
}
