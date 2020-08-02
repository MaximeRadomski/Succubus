using UnityEngine;

public class PoppingTextBhv : MonoBehaviour
{
    public Vector2 StartingPosition;
    public TMPro.TMP_FontAsset[] Fonts;

    private TMPro.TextMeshPro _text;
    private SpriteRenderer _shadow;
    private string _material;
    private bool _isMoving;
    private Vector2 _positionToReach;
    private Color _colorToFade;


    public void SetPrivates(string text, Vector2 startingPosition, TextType type, TextThickness thickness)
    {
        if (type == TextType.HpCritical)
            text += "!";
        StartingPosition = startingPosition;
        transform.position = new Vector2(startingPosition.x, startingPosition.y);
        _positionToReach = new Vector2(startingPosition.x, startingPosition.y + 0.25f);
        _text = GetComponent<TMPro.TextMeshPro>();
        _text.font = thickness == TextThickness.Thick ? Fonts[0] : Fonts[1];
        _material = Helper.MaterialFromTextType(type.GetHashCode(), thickness);
        _text.text = "<material=\"" + _material + "\">" + text + "</material>";
        _colorToFade = new Color(_text.color.r, _text.color.g, _text.color.b, 0.0f);
        _shadow = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _shadow.transform.localScale = new Vector3(_text.preferredWidth + 0.15f, _text.preferredHeight - 0.15f, 1.0f);
        _isMoving = true;
    }

    private void Update()
    {
        if (_isMoving)
            MoveAndFade();
    }

    private void MoveAndFade()
    {
        transform.position = Vector2.Lerp(transform.position, _positionToReach, 0.05f);
        if (transform.position.y >= _positionToReach.y - 0.05f)
        {
            //tag = Constants.TagCell;
            _text.color = Color.Lerp(_text.color, _colorToFade, 0.1f);
            _shadow.color = Color.Lerp(_shadow.color, Constants.ColorPlainTransparent, 0.1f);
            if (_text.color.a <= 0.01)
            {
                _isMoving = false;
                Destroy(gameObject);
            }
        }
    }
}
