using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramesAnimationBhv : MonoBehaviour
{
    public List<Sprite> Sprites;
    public float Delay = 0.30f;

    private int _idSprite = 0;
    private float _lastSpriteTime = -1.0f;
    private bool _isEnabled = false;
    private Action _afterAnimation;
    private SpriteRenderer _spriteRenderer;

    public void Init(Action afterAnimation)
    {
        _afterAnimation = afterAnimation;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _isEnabled = true;
    }

    void Update()
    {
        if (!_isEnabled)
            return;
        HandleSprites();
    }

    private void HandleSprites()
    {
        if (Time.time > _lastSpriteTime + Delay)
        {            
            if (_idSprite < Sprites.Count)
            {
                _spriteRenderer.sprite = Sprites[_idSprite];
                _lastSpriteTime = Time.time;
                ++_idSprite;
            }
            else
            {
                _isEnabled = false;
                _afterAnimation?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
