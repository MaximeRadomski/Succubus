using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastBhv : FrameRateBehavior
{
    private SpriteRenderer _backgroundRenderer;
    private TMPro.TextMeshPro _textRenderer;
    private int _state = 0;
    private float _duration;
    private float _verticalOffset = 10.0f;
    private Vector3 _targetPosition;
    private Vector3 _originPosition;
    private float _speed = 0.15f;
    private bool hasDecreasedInputLayer = false;

    public void Init(string content, float duration = 1.5f)
    {
        _duration = duration;
        var contentGO = transform.Find("Content").gameObject;
        _textRenderer = contentGO.GetComponent<TMPro.TextMeshPro>();
        _textRenderer.text = content;
        _textRenderer.color = Constants.ColorPlainTransparent;
        _backgroundRenderer = gameObject.GetComponent<SpriteRenderer>();
        _backgroundRenderer.color = Constants.ColorPlainTransparent;
        var buttonBhv = _backgroundRenderer.GetComponent<ButtonBhv>();
        buttonBhv.EndActionDelegate = ExitToast;
        buttonBhv.IsMenuSelectorResetButton = true;
        var positionBhv = gameObject.GetComponent<PositionBhv>();
        positionBhv.UpdatePositions();
        InvokeNextFrame(() =>
        {
            _originPosition = transform.position;
            _targetPosition = new Vector3(transform.position.x, transform.position.y + _verticalOffset, 0.0f);
            _state = 1;
        });
    }

    protected override void FrameUpdate()
    {
        if (_state == 1)
            Toasting();
        else if (_state == 2)
        {
            _state = 3;
            Invoke(nameof(StartFading), _duration);
        }
        else if (_state == 4)
            Fading();
    }

    private void Toasting()
    {
        
        _backgroundRenderer.color = Color.Lerp(_backgroundRenderer.color, Constants.ColorPlain, _speed);
        _textRenderer.color = Color.Lerp(_textRenderer.color, Constants.ColorPlain, _speed);
        transform.position = Vector3.Lerp(transform.position, _targetPosition, _speed);
        if (Helper.FloatEqualsPrecision(transform.position.y, _targetPosition.y, 0.005f))
        {
            ++_state;
            _backgroundRenderer.color = Constants.ColorPlain;
            _textRenderer.color = Constants.ColorPlain;
            transform.position = _targetPosition;
        }
    }

    private void StartFading()
    {
        _state = 4;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<ButtonBhv>().enabled = false;
        hasDecreasedInputLayer = true;
        Cache.DecreaseInputLayer();
    }

    private void Fading()
    {
        _backgroundRenderer.color = Color.Lerp(_backgroundRenderer.color, Constants.ColorPlainTransparent, _speed);
        _textRenderer.color = Color.Lerp(_textRenderer.color, Constants.ColorPlainTransparent, _speed);
        transform.position = Vector3.Lerp(transform.position, _originPosition, _speed);
        if (Helper.FloatEqualsPrecision(transform.position.y, _originPosition.y, 0.005f))
        {
            ++_state;
            ExitToast();
        }
    }

    public void ExitToast()
    {
        _state = 4;
        if (!hasDecreasedInputLayer)
            Cache.DecreaseInputLayer();
        Destroy(gameObject);
    }
}
