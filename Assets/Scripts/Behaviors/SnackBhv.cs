using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackBhv : MonoBehaviour
{
    private int _state;
    private SpriteRenderer _spriteRenderer;
    private TMPro.TextMeshPro _textMesh;
    private float _duration;

    public void SetPrivates(string content, float duration)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = Constants.ColorPlainTransparent;
        _textMesh = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _textMesh.color = Constants.ColorPlainTransparent;
        _textMesh.text = content;
        _duration = duration;
        _state = 0;
    }


    void Update()
    {
        if (_state == 0)
        {
            Invoke("Disapear", _duration);
            _state = 1;
        }
        else if (_state == 1)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlain, 0.1f);
            _textMesh.color = Color.Lerp(_textMesh.color, Constants.ColorPlain, 0.1f);
            if (_spriteRenderer.color == Constants.ColorPlainTransparent)
                _state = 2;
        }
        else if (_state == 3)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlainTransparent, 0.1f);
            _textMesh.color = Color.Lerp(_textMesh.color, Constants.ColorPlainTransparent, 0.1f);
            if (_spriteRenderer.color == Constants.ColorPlainTransparent)
                Destroy(gameObject);
        }
    }

    private void Disapear()
    {
        _state = 3;
    }
}
