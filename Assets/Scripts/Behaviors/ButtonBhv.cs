﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBhv : InputBhv
{
    public delegate void ActionDelegate();
    public ActionDelegate BeginActionDelegate;
    public ActionDelegate DoActionDelegate;
    public ActionDelegate EndActionDelegate;
    public ActionDelegate LongPressActionDelegate;
    public bool Disabled;
    public bool StretchDisabled;
    public bool ColorDisabled;
    public bool CustomSound;
    public bool VibratesOnClick = false;
    public bool AlterChildren = true;
    public float ConeVisionMult = Constants.BaseButtonVisionConeMult;
    public bool IsMenuSelectorResetButton;

    private SpriteRenderer _spriteRenderer;
    private SoundControlerBhv _soundControler;

    private bool _isStretching;
    private Vector3 _resetedScale;
    private Vector3 _pressedScale;
    private bool _isResetingColor;
    private Color _resetedColor;
    private Color _pressedColor;
    private float? _beginPress = null;
    private float _longPressDelay = 0.35f;
    private bool _hasDoneLongPress = false;

    void Start()
    {
        SetPrivates();
    }

    public override void SetPrivates()
    {
        base.SetPrivates();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();

        _isStretching = false;
        _resetedScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _pressedScale = new Vector3(transform.localScale.x * 1.2f, transform.localScale.y * 1.1f, transform.localScale.z * 1.0f);
        _isResetingColor = false;
        if (_spriteRenderer == null)
            return;
        _resetedColor = Disabled ? Constants.ColorPlain : _spriteRenderer.color;
        _pressedColor = new Color(_spriteRenderer.color.r * 0.7f, _spriteRenderer.color.g * 0.7f, _spriteRenderer.color.b * 0.7f, 1.0f);
    }

    public void SetResetedColor(Color color)
    {
        _resetedColor = color;
    }

    public override void BeginAction(Vector2 initialTouchPosition)
    {
        _beginPress = Time.time;
        _hasDoneLongPress = false;
        if (_soundControler == null)
            SetPrivates();
        if (!CustomSound)
            _soundControler.PlaySound(_soundControler.ClickIn);
        if (VibratesOnClick)
            VibrationService.Vibrate(30);
        if (!StretchDisabled)
        {
            _isStretching = true;
            transform.localScale = _pressedScale;
        }
        _isResetingColor = false;
        if (_spriteRenderer != null && !ColorDisabled)
        {
            _spriteRenderer.color = _pressedColor;
            if (AlterChildren)
                foreach (Transform child in this.transform)
                {
                    if (child.TryGetComponent<SpriteRenderer>(out var childRenderer))
                    {
                        childRenderer.color = _pressedColor;
                    }
                }
        }
        BeginActionDelegate?.Invoke();
    }

    public override void DoAction(Vector2 touchPosition)
    {
        DoActionDelegate?.Invoke();
        if (LongPressActionDelegate != null && _beginPress != null && Time.time - _beginPress > _longPressDelay)
        {
            _beginPress = null;
            _hasDoneLongPress = true;
            _isResetingColor = true;
            LongPressActionDelegate?.Invoke();
        }
    }

    public override void EndAction(Vector2 lastTouchPosition)
    {
        if (!CustomSound)
            _soundControler.PlaySound(_soundControler.ClickOut);
        _beginPress = null;
        _isResetingColor = true;
        if (_hasDoneLongPress == false)
            EndActionDelegate?.Invoke();
    }

    public override void CancelAction()
    {
        _beginPress = null;
        _isResetingColor = true;
    }

    override protected void FrameUpdate()
    {
        if (_isStretching)
            StretchOnBegin();
        if (_isResetingColor && !ColorDisabled)
            ResetColorOnEnd();
    }

    private void StretchOnBegin()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _resetedScale, 0.2f);
        if (transform.localScale == _resetedScale)
            _isStretching = false;
    }

    private void ResetColorOnEnd()
    {
        if (_spriteRenderer == null)
        {
            _isResetingColor = false;
            return;
        }
        if (Disabled)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlainSemiTransparent, 0.1f);
            if (AlterChildren)
                foreach (Transform child in this.transform)
                {
                    if (child.TryGetComponent<SpriteRenderer>(out var childRenderer))
                    {
                        childRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlainSemiTransparent, 0.1f);
                    }
                }
            if (_spriteRenderer.color == Constants.ColorPlainSemiTransparent)
                _isResetingColor = false;
        }
        else
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _resetedColor, 0.1f);
            if (AlterChildren)
                foreach (Transform child in this.transform)
                {
                    if (child.TryGetComponent<SpriteRenderer>(out var childRenderer))
                    {
                        childRenderer.color = Color.Lerp(_spriteRenderer.color, _resetedColor, 0.1f);
                    }
                }
            if (_spriteRenderer.color == _resetedColor)
                _isResetingColor = false;
        }
    }

    public void DisableButton()
    {
        Disabled = true;
        if (_spriteRenderer == null)
            SetPrivates();
        if (_spriteRenderer != null)
            _spriteRenderer.color = Constants.ColorPlainSemiTransparent;
        GetComponent<BoxCollider2D>().enabled = false;
        for (int i = 0; i < transform.childCount; ++i)
        {
            var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            var textMesh = transform.GetChild(i).GetComponent<TMPro.TextMeshPro>();
            if (spriteRenderer != null) spriteRenderer.color = Constants.ColorPlainSemiTransparent;
            if (textMesh != null) textMesh.color = Constants.ColorPlainSemiTransparent;
            var boxCollider = transform.GetChild(i).GetComponent<BoxCollider2D>();
            if (boxCollider != null)
                boxCollider.enabled = false;
        }
    }

    public void EnableButton()
    {
        Disabled = false;
        if (_spriteRenderer == null)
            return;
        _spriteRenderer.color = Constants.ColorPlain;
        GetComponent<BoxCollider2D>().enabled = true;
        for (int i = 0; i < transform.childCount; ++i)
        {
            var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            var textMesh = transform.GetChild(i).GetComponent<TMPro.TextMeshPro>();
            if (spriteRenderer != null) spriteRenderer.color = Constants.ColorPlain;
            if (textMesh != null) textMesh.color = Constants.ColorPlain;
            var boxCollider = transform.GetChild(i).GetComponent<BoxCollider2D>();
            if (boxCollider != null)
                boxCollider.enabled = false;
        }
    }
}
