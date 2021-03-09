using UnityEngine;

public class PoppingIconBhv : FrameRateBehavior
{
    public Vector2 StartingPosition;
    
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _shadow;
    private bool _isMoving;
    private Vector2 _positionToReach;
    private Color _colorToFade;

    public void Init(Sprite sprite, Vector2 startingPosition)
    {
        StartingPosition = startingPosition;
        transform.position = new Vector2(startingPosition.x, startingPosition.y);
        _positionToReach = new Vector2(startingPosition.x, startingPosition.y + 0.25f);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = sprite;
        _colorToFade = Constants.ColorPlainTransparent;
        _shadow = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _shadow.transform.localScale = new Vector3(_spriteRenderer.sprite.rect.width * Constants.Pixel + 0.15f, _spriteRenderer.sprite.rect.height * Constants.Pixel + 0.15f, 1.0f);
        _isMoving = true;
    }

    protected override void FrameUpdate()
    {
        if (_isMoving)
            MoveAndFade();
    }

    private void MoveAndFade()
    {
        transform.position = Vector2.Lerp(transform.position, _positionToReach, 0.05f);
        if (transform.position.y >= _positionToReach.y - 0.05f)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _colorToFade, 0.1f);
            _shadow.color = Color.Lerp(_shadow.color, Constants.ColorPlainTransparent, 0.1f);
            if (_spriteRenderer.color.a <= 0.01)
            {
                _isMoving = false;
                Destroy(gameObject);
            }
        }
    }
}
