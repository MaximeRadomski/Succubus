using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnAppearanceBhv : FrameRateBehavior
{
    public bool SelfInit;
    public float Speed;
    public Color FadeColor;

    private List<SpriteRenderer> _renderers;
    private TMPro.TextMeshPro _textRenderer;

    private bool _isFading;

    private void Start()
    {
        if (SelfInit)
            Init(Speed, FadeColor);
    }

    public void Init(float speed, Color? color, bool recursiveChildren = false)
    {
        _renderers = new List<SpriteRenderer>();
        if (!recursiveChildren)
            _renderers.Add(gameObject.GetComponent<SpriteRenderer>());
        else
        {
            foreach (Transform child in this.transform)
                _renderers.Add(child.GetComponent<SpriteRenderer>());
        }
        _textRenderer = gameObject.GetComponent<TMPro.TextMeshPro>();
        if (color != null && _renderers != null && _renderers.Count > 0)
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                    renderer.color = color.Value;
            }
        if (color != null && _textRenderer != null)
            _textRenderer.color = color.Value;
        if (_renderers != null && _renderers.Count > 0 && _renderers[0] != null)
            FadeColor = new Color(_renderers[0].color.r, _renderers[0].color.g, _renderers[0].color.b, 0.0f);
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
        if (_renderers != null && _renderers.Count > 0 && _renderers[0] != null)
            foreach (var renderer in _renderers)
                renderer.color = Color.Lerp(_renderers[0].color, FadeColor, Speed);
        if (_textRenderer != null)
            _textRenderer.color = Color.Lerp(_textRenderer.color, FadeColor, Speed);
        if (_renderers != null && _renderers.Count > 0 && _renderers[0] != null && Helper.FloatEqualsPrecision(_renderers[0].color.a, FadeColor.a, 0.005f))
        {
            _isFading = false;
            foreach (var renderer in _renderers)
                renderer.color = FadeColor;
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
