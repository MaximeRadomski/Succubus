using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBhv : InputBhv
{
    public delegate void ActionDelegate();
    public ActionDelegate BeginActionDelegate;
    public ActionDelegate DoActionDelegate;
    public ActionDelegate EndActionDelegate;
    public bool Disabled;
    public bool StretchDisabled;
    public bool CustomSound;
    public float ConeVisionMult = Constants.BaseButtonVisionConeMult;

    private SpriteRenderer _spriteRenderer;
    private SoundControlerBhv _soundControler;

    private bool _isStretching;
    private Vector3 _resetedScale;
    private Vector3 _pressedScale;
    private bool _isResetingColor;
    private Color _resetedColor;
    private Color _pressedColor;

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
        if (_soundControler == null)
            SetPrivates();
        if (!CustomSound)
            _soundControler.PlaySound(_soundControler.ClickIn);
        if (!StretchDisabled)
        {
            _isStretching = true;
            transform.localScale = _pressedScale;
        }
        _isResetingColor = false;
        if (_spriteRenderer != null)
            _spriteRenderer.color = _pressedColor;
        BeginActionDelegate?.Invoke();
    }

    public override void DoAction(Vector2 touchPosition)
    {
        DoActionDelegate?.Invoke();
    }

    public override void EndAction(Vector2 lastTouchPosition)
    {
        if (!CustomSound)
            _soundControler.PlaySound(_soundControler.ClickOut);
        _isResetingColor = true;
        EndActionDelegate?.Invoke();
    }

    public override void CancelAction()
    {
        _isResetingColor = true;
    }

    private void Update()
    {
        if (_isStretching)
            StretchOnBegin();
        if (_isResetingColor)
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
            if (_spriteRenderer.color == Constants.ColorPlainSemiTransparent)
                _isResetingColor = false;
        }
        else
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _resetedColor, 0.1f);
            if (_spriteRenderer.color == _resetedColor)
                _isResetingColor = false;
        }
    }

    public void DisableButton()
    {
        Disabled = true;
        if (_spriteRenderer == null)
            SetPrivates();
        if (_spriteRenderer == null)
            return;
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
