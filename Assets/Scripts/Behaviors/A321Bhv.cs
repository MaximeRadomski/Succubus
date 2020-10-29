using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A321Bhv : MonoBehaviour
{
    public List<Sprite> Sprites;

    private int _idSprite = 0;
    private float _lastSpriteTime = -1.0f;
    private bool _isEnabled = false;
    private Func<bool> _afterAnimation;
    private SpriteRenderer _spriteRenderer;

    public void Init(Func<bool> afterAnimation)
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
        if (Time.time > _lastSpriteTime + 0.30f)
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
                _afterAnimation();
                Destroy(gameObject);
            }
        }
    }
}
