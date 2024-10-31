using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnInvokeBhv : MonoBehaviour
{
    public Sprite Sprite;
    public Color TextColor;

    private SpriteRenderer _spriteRenderer;
    private TextMeshPro _textMeshPro;

    void Start()
    {
        bool hasInvoke = PlayerPrefsHelper.GetPhillHasBeenInvoked();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _textMeshPro = GetComponent<TextMeshPro>();

        if (hasInvoke)
        {
            if (_spriteRenderer != null && Sprite != null)
            {
                _spriteRenderer.sprite = Sprite;
            }
            if (_textMeshPro != null && TextColor != null)
            {
            }
        }
        
    }
}
