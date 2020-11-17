using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBhv : MonoBehaviour
{
    public GameObject Shadow;
    public bool IsMimicked;

    private SpriteRenderer _spriteRenderer;
    private Vector3 _originalLocalPosition;
    private Vector3 _targetLocalPosition;
    private Color _originalColor;
    private Color _spreadToColor;
    private float _opacityGap = 0.5f;
    private bool _spreading;
    private bool _resetingSpread;
    private bool _isPoundering;
    private bool _isResetingPosition;

    public void Start()
    {
        Shadow = transform.GetChild(0).gameObject;
        Shadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        Shadow.GetComponent<SpriteRenderer>().sortingOrder = -2;
        _originalLocalPosition = transform.localPosition;
    }

    public void CastShadow()
    {
        if (Shadow != null)
            Shadow.transform.position = transform.position + new Vector3(Constants.Pixel, Constants.Pixel, 0.0f);
    }

    public void SetMimicAppearance()
    {
        IsMimicked = true;
        GetComponent<SpriteRenderer>().color = Constants.ColorPlainQuarterTransparent;
        Shadow.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
    }

    public void UnsetMimicAppearance(float maxOpacity)
    {
        IsMimicked = false;
        var plainColor = new Color(Constants.ColorPlain.r, Constants.ColorPlain.g, Constants.ColorPlain.b, maxOpacity);
        var shadowColor = new Color(Constants.ColorBlack.r, Constants.ColorBlack.g, Constants.ColorBlack.b, maxOpacity);
        GetComponent<SpriteRenderer>().color = plainColor;
        Shadow.GetComponent<SpriteRenderer>().color = shadowColor;
    }

    public void PreventYAxisShif()
    {
        int roundedY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector3(transform.position.x, roundedY, 0.0f);
    }

    private void Update()
    {
        if (_spreading)
            Spreading();
        else if (_resetingSpread)
            ResetingSpread();
        if (_isPoundering)
            Poundering();
        else if (_isResetingPosition)
            ResetingPosition();
    }

    public void Spread()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            return;
        _originalColor = _spriteRenderer.color;
        _spreadToColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, _originalColor.a - _opacityGap);
        _spreading = true;
    }

    private void Spreading()
    {
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _spreadToColor, 0.2f);
        if (Helper.FloatEqualsPrecision(_spriteRenderer.color.a, _spreadToColor.a, 0.05f))
        {
            _spreading = false;
            _resetingSpread = true;
        }
    }

    private void ResetingSpread()
    {
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _originalColor, 0.05f);
        if (Helper.FloatEqualsPrecision(_spriteRenderer.color.a, _spreadToColor.a, 0.01f))
        {
            _spriteRenderer.color = _originalColor;
            _resetingSpread = false;
        }
    }

    public void Pounder()
    {
        _targetLocalPosition = transform.localPosition + new Vector3(0.0f, Constants.Pixel * 2, 0.0f);
        _isResetingPosition = false;
        _isPoundering = true;
    }

    private void Poundering()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetLocalPosition, 0.25f);
        if (Helper.FloatEqualsPrecision(transform.localPosition.y, _targetLocalPosition.y, 0.05f))
        {
            _isPoundering = false;
            _isResetingPosition = true;
        }
    }

    private void ResetingPosition()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, _originalLocalPosition, 0.1f);
        if (Helper.FloatEqualsPrecision(transform.localPosition.y, _originalLocalPosition.y, 0.05f))
        {
            transform.localPosition = _originalLocalPosition;
            _isResetingPosition = false;
        }
    }
}
