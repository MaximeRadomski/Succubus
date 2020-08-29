﻿using UnityEngine;

public class ResourceBarBhv : MonoBehaviour
{
    private TMPro.TextMeshPro _text;
    private GameObject _content;
    private GameObject _subContent;
    private SpriteRenderer _contentSpriteRenderer;
    private float _width;
    private float? _frameHeight;
    private bool _isSet;

    GameObject _instantChange;
    GameObject _delayedChange;
    private bool _isDelayingContent;
    private float _delayingSpeed;
    private bool _tiltingUp;

    void Start()
    {
        if (!_isSet)
            Init();
    }

    void Update()
    {
        if (_isDelayingContent)
        {
            _delayedChange.transform.localScale = Vector3.Lerp(_delayedChange.transform.localScale, _instantChange.transform.localScale, _delayingSpeed);
            _delayedChange.transform.position = Vector3.Lerp(_delayedChange.transform.position, _instantChange.transform.position, _delayingSpeed);
            _delayingSpeed *= 1.2f;
            if (Helper.VectorEqualsPrecision(_delayedChange.transform.position, _instantChange.transform.position, 0.005f))
            {
                _delayedChange.transform.localScale = _instantChange.transform.localScale;
                _delayedChange.transform.position = _instantChange.transform.position;
                _isDelayingContent = false;
            }
        }
    }

    private void Init()
    {
        _text = transform.Find("Text")?.GetComponent<TMPro.TextMeshPro>();
        _content = transform.Find("Content")?.gameObject;
        _subContent = transform.Find("SubContent")?.gameObject;
        _contentSpriteRenderer = _content.GetComponent<SpriteRenderer>();
        _width = _contentSpriteRenderer.sprite.rect.size.x * Constants.Pixel;
        _isSet = true;
    }

    public void UpdateContent(int current, int max, Direction direction = Direction.None)
    {
        bool isDelaying;
        if (current < 0)
            current = 0;
        if (current > max)
            current = max;
        if (_text == null && _content == null)
            Init();
        if (direction == Direction.Up)
        {
            _instantChange = _subContent;
            _delayedChange = _content;
            isDelaying = true;
        }
        else if (direction == Direction.Down)
        {
            _instantChange = _content;
            _delayedChange = _subContent;
            isDelaying = true;
        }
        else
        {
            _instantChange = _content;
            _delayedChange = _subContent;
            isDelaying = false;
        }

        if (_text != null)
            _text.text = current.ToString();
        if (isDelaying)
        {
            _delayedChange.transform.localScale = _instantChange.transform.localScale;
            _delayedChange.transform.position = _instantChange.transform.position;
            _isDelayingContent = true;
            _delayingSpeed = 0.0001f;
        }
        float ratio = (float)current / max;
        _instantChange.transform.localScale = new Vector3(1.0f * ratio, 1.0f, 1.0f);
        var space = _width * ((1.0f - ratio) / 2);
        _instantChange.transform.position = new Vector3(transform.position.x - space, transform.position.y, 0.0f);
        if (!isDelaying)
        {
            _delayedChange.transform.localScale = _instantChange.transform.localScale;
            _delayedChange.transform.position = _instantChange.transform.position;
        }
    }

    public void Tilt()
    {
        if (_tiltingUp == false)
        {
            _contentSpriteRenderer.color = Color.Lerp(_contentSpriteRenderer.color, Constants.ColorPlainTransparent, 0.3f);
            if (Helper.FloatEqualsPrecision(_contentSpriteRenderer.color.a, Constants.ColorPlainTransparent.a, 0.1f))
            {
                _contentSpriteRenderer.color = Constants.ColorPlainTransparent;
                _tiltingUp = true;
            }
        }
        else
        {
            _contentSpriteRenderer.color = Color.Lerp(_contentSpriteRenderer.color, Constants.ColorPlain, 0.3f);
            if (Helper.FloatEqualsPrecision(_contentSpriteRenderer.color.a, Constants.ColorPlain.a, 0.1f))
            {
                _contentSpriteRenderer.color = Constants.ColorPlain;
                _tiltingUp = false;
            }
        }
    }

    public void ResetTilt()
    {
        _contentSpriteRenderer.color = Constants.ColorPlain;
        _tiltingUp = false;
    }
}
