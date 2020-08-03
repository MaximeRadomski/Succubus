using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FadeBlockBhv : MonoBehaviour
{
    public Nature Nature;

    private SpriteRenderer _spriteRenderer;
    private float _delay;
    private float _delayRange;
    private int _sequence;
    private bool _increment;
    private int _endColor;

    //5 = White
    //-1 = Black

    public void Init(int startColor, int endColor)
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _delay = 0.05f;
        _delayRange = 0.5f;
        _sequence = startColor;
        _increment = startColor < endColor;
        _endColor = endColor;
        LoopColor();
    }

    private void LoopColor()
    {
        Color tmpColor = Color.white;
        if (_sequence >= 0 && _sequence <= 4)
            tmpColor = (Color)Constants.GetColorFromNature(Nature, _sequence);
        else if (_sequence < 0)
            tmpColor = Color.black;
        _spriteRenderer.color = tmpColor;
        if (_increment)
            ++_sequence;
        else
            --_sequence;
        if ((_increment && _sequence <= _endColor)
            || (!_increment && _sequence >= _endColor))
            Invoke(nameof(LoopColor), Random.Range(_delay - _delay * _delayRange, _delay + _delay * _delayRange));
        else
            Invoke(nameof(DestroyAfterDelay), _delay);
    }

    private void DestroyAfterDelay()
    {
        Destroy(gameObject);
    }
}
