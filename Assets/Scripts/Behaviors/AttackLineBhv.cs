using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class AttackLineBhv : FrameRateBehavior
{
    private Realm _realm;
    private Instantiator _instantiator;
    private Vector3 _target;
    private float _distance;
    private float _distanceToAdd;
    private Vector3 _fadeBlockScale;
    private Sprite _sprite;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _originalScale;
    private Vector3 _poppingScale;

    private bool _reachingTarget;
    private bool _popping;
    private bool _resetPopping;
    private bool _linear;

    private Action _onPop;

    public void Init(Vector3 source, Vector3 target, Realm realm, Instantiator instantiator, bool linear, Sprite sprite, Action onPop)
    {
        _target = target;
        transform.position = source + new Vector3(0.0f, -2.0f, 0.0f);
        if (transform.position.y < 0)
            transform.position = new Vector3(transform.position.x, 0.0f, 0.0f);
        _realm = realm;
        _linear = linear;
        _instantiator = instantiator;
        if (linear)
            _distance = _distanceToAdd = 0.05f;
        else
            _distance = 0.35f;
        _fadeBlockScale = new Vector3(0.75f, 0.75f, 0.75f);
        _sprite = sprite;
        if (_sprite != null)
        {
            _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            _spriteRenderer.sprite = sprite;
            _originalScale = transform.localScale;
            _poppingScale = new Vector3(1.5f, 1.5f, 1.0f);
            _onPop = onPop;
        }
        _reachingTarget = true;
    }

    protected override void FrameUpdate()
    {
        if (_target == null)
            return;

        if (_reachingTarget)
            ReachTarget();
        else if (_popping)
            Popping();
        else if (_resetPopping)
            ResetPopping();
    }

    private void ReachTarget()
    {
        var fadeBlock = _instantiator.NewFadeBlock(_realm, transform.position, 4, 0);
        fadeBlock.GetComponent<SpriteRenderer>().sortingLayerName = "Effects";
        fadeBlock.transform.localScale = _fadeBlockScale;

        fadeBlock = _instantiator.NewFadeBlock(_realm, Vector3.Lerp(transform.position, _target, _distance / 2), 4, 0);
        fadeBlock.GetComponent<SpriteRenderer>().sortingLayerName = "Effects";
        fadeBlock.transform.localScale = _fadeBlockScale;

        transform.position = Vector3.Lerp(transform.position, _target, _distance);
        if (Helper.VectorEqualsPrecision(transform.position, _target, 0.1f))
            OnTargetReached();
        if (_linear)
            _distance += _distanceToAdd;
    }

    private void OnTargetReached()
    {
        _reachingTarget = false;
        if (_sprite != null)
            _popping = true;
        else
            Destroy(gameObject);
    }

    private void Popping()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _poppingScale, 0.5f);
        if (Helper.VectorEqualsPrecision(transform.localScale, _poppingScale, 0.01f))
        {
            transform.localScale = _poppingScale;
            _popping = false;
            _resetPopping = true;
            _onPop?.Invoke();
        }
    }

    private void ResetPopping()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.3f);
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlainTransparent, 0.35f);
        if (Helper.VectorEqualsPrecision(transform.localScale, _originalScale, 0.01f) && Helper.FloatEqualsPrecision(_spriteRenderer.color.a, Constants.ColorPlainTransparent.a, 0.01f))
        {
            transform.localScale = _originalScale;
            _spriteRenderer.color = Constants.ColorPlainTransparent;
            _resetPopping = false;
            Destroy(gameObject);
        }
    }
}