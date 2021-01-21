using UnityEngine;

public class PoppingTextBhv : MonoBehaviour
{
    public Vector2 StartingPosition;

    private TMPro.TextMeshPro _text;
    private bool _isMoving;
    private Vector2 _positionToReach;
    private Color _colorToFade;
    private float _floatingTime;
    private float _speed;
    private float _startFadingDistance;
    private float _fadingSpeed;

    public void Init(string text, Vector2 startingPosition, float floatingTime, float speed, float distance, float startFadingDistancePercent, float fadingSpeed)
    {
        StartingPosition = startingPosition;
        transform.position = new Vector2(startingPosition.x, startingPosition.y);
        _positionToReach = new Vector2(startingPosition.x, startingPosition.y + distance);
        _text = GetComponent<TMPro.TextMeshPro>();
        _text.text = text;
        _colorToFade = new Color(_text.color.r, _text.color.g, _text.color.b, 0.0f);
        _floatingTime = Time.time + floatingTime;
        var nbPoppingTexts = GameObject.FindGameObjectsWithTag(Constants.TagPoppingText);
        _text.sortingOrder = nbPoppingTexts.Length + 1;
        _speed = speed;
        _startFadingDistance = distance * startFadingDistancePercent;
        _fadingSpeed = fadingSpeed;
        _isMoving = true;
    }

    private void Update()
    {
        if (_isMoving)
            MoveAndFade();
    }

    private void MoveAndFade()
    {
        transform.position = Vector2.Lerp(transform.position, _positionToReach, _speed);
        if (transform.position.y >= _positionToReach.y - _startFadingDistance && Time.time >= _floatingTime)
        {
            //tag = Constants.TagCell;
            _text.color = Color.Lerp(_text.color, _colorToFade, _fadingSpeed);
            if (_text.color.a <= 0.01)
            {
                _isMoving = false;
                Destroy(gameObject);
            }
        }
    }
}
