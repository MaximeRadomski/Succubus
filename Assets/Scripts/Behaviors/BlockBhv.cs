using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBhv : FrameRateBehavior
{
    public GameObject Shadow;
    public bool IsMimicked;
    public bool Indestructible;

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Color _spreadToColor;
    private float _opacityGap = 0.25f;
    private float _opacityGapSpreadDown;
    private bool _spreading;
    private bool _resetingSpread;
    private float _spreadSpeed;
    private int[] _coord = { -1, -1 };
    private bool _hasSpreadDown;
    private GameplayControler _gameplayControler;

    public void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (transform.childCount > 0)
        {
            Shadow = transform.GetChild(0).gameObject;
            var shadowRenderer = Shadow.GetComponent<SpriteRenderer>();
            shadowRenderer.sprite = _spriteRenderer.sprite;
            shadowRenderer.sortingOrder = -2;
            shadowRenderer.flipX = _spriteRenderer.flipX;
            shadowRenderer.flipY = _spriteRenderer.flipY;
            shadowRenderer.transform.rotation = _spriteRenderer.transform.rotation;
        }
        _hasSpreadDown = false;
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
        if (Shadow != null)
            Shadow.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
    }

    public void UnsetMimicAppearance(float maxOpacity)
    {
        IsMimicked = false;
        var plainColor = new Color(Constants.ColorPlain.r, Constants.ColorPlain.g, Constants.ColorPlain.b, maxOpacity);
        var shadowColor = new Color(Constants.ColorBlack.r, Constants.ColorBlack.g, Constants.ColorBlack.b, maxOpacity);
        GetComponent<SpriteRenderer>().color = plainColor;
        if (Shadow != null)
            Shadow.GetComponent<SpriteRenderer>().color = shadowColor;
    }

    public void PreventYAxisShif()
    {
        int roundedY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector3(transform.position.x, roundedY, 0.0f);
    }

    override protected void FrameUpdate()
    {
        if (_spreading)
            Spreading();
        else if (_resetingSpread)
            ResetingSpread();
    }

    public void Spread(float spreadSpeed, int x, int y, GameplayControler gameplayControler)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            return;
        _spreadSpeed = spreadSpeed;
        if (_spreading == false && _resetingSpread == false)
            _originalColor = _spriteRenderer.color;
        _spreadToColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, _originalColor.a - _opacityGap);
        _opacityGapSpreadDown = _originalColor.a - (_opacityGap / 2.0f);
        _spreading = true;
        _hasSpreadDown = false;
        _coord[0] = x;
        _coord[1] = y;
        _gameplayControler = gameplayControler;
    }

    private void Spreading()
    {
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _spreadToColor, _spreadSpeed);
        if (!_hasSpreadDown && _spriteRenderer.color.a <= _opacityGapSpreadDown)
            SpreadDown();
        if (Helper.FloatEqualsPrecision(_spriteRenderer.color.a, _spreadToColor.a, 0.01f))
        {
            _spreading = false;
            _resetingSpread = true;
        }
    }

    private void SpreadDown()
    {
        _hasSpreadDown = true;
        if (_coord[1] - 1 < 0)
            return;
        var underBlockTransform = _gameplayControler.PlayFieldBhv.Grid[_coord[0], _coord[1] - 1];
        if (underBlockTransform == null)
            return;
        var underBlockBhv = underBlockTransform?.GetComponent<BlockBhv>();
        if (underBlockBhv == null)
            return;
        underBlockBhv.Spread(_spreadSpeed, _coord[0], _coord[1] - 1, _gameplayControler);
    }

    private void ResetingSpread()
    {
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _originalColor, _spreadSpeed / 4);
        if (Helper.FloatEqualsPrecision(_spriteRenderer.color.a, _originalColor.a, 0.01f))
        {
            _spriteRenderer.color = _originalColor;
            _resetingSpread = false;
        }
    }
}
