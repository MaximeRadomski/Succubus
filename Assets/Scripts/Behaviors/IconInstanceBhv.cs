using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconInstanceBhv : FrameRateBehavior
{
    private SpriteRenderer _spriteRenderer;
    
    private Vector3 _originalScale;
    private Vector3 _poppingScale;
    private bool _popping;
    private bool _resetPopping;

    private bool _hasInit;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_hasInit)
            return;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
        _poppingScale = new Vector3(1.3f, 1.5f, 1.0f);
        _hasInit = true;
    }

    override protected void FrameUpdate()
    {
        if (_popping)
            Popping();
        else if (_resetPopping)
            ResetPopping();
    }

    public void SetVisible(bool visible)
    {
        if (!_hasInit)
            Init();
        _spriteRenderer.enabled = visible;
    }

    public void SetSkin(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void Pop(float? customX = null, float? customY = null)
    {
        if (customX != null)
            _poppingScale = new Vector3(customX.Value, _poppingScale.y, _poppingScale.z);
        if (customY != null)
            _poppingScale = new Vector3(_poppingScale.x, customY.Value, _poppingScale.z);
        _popping = true;
    }

    private void Popping()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _poppingScale, 0.5f);
        if (Helper.VectorEqualsPrecision(transform.localScale, _poppingScale, 0.01f))
        {
            transform.localScale = _poppingScale;
            _popping = false;
            _resetPopping = true;
        }
    }

    private void ResetPopping()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.2f);
        if (Helper.VectorEqualsPrecision(transform.localScale, _originalScale, 0.01f))
        {
            transform.localScale = _originalScale;
            _resetPopping = false;
        }
    }
}
