using UnityEngine;

public class PoppingTextBhv : MonoBehaviour
{
    public Vector2 StartingPosition;

    private TMPro.TextMeshPro _text;
    private bool _isMoving;
    private Vector2 _positionToReach;
    private Color _colorToFade;
    private float _floatingTime;


    public void Init(string text, Vector2 startingPosition, string color, float floatingTime)
    {
        StartingPosition = startingPosition;
        transform.position = new Vector2(startingPosition.x, startingPosition.y);
        _positionToReach = new Vector2(startingPosition.x, startingPosition.y + 0.25f);
        _text = GetComponent<TMPro.TextMeshPro>();
        _text.text = "<color=" + color + ">" + text + "</color>";
        _colorToFade = new Color(_text.color.r, _text.color.g, _text.color.b, 0.0f);
        _floatingTime = Time.time + floatingTime;
        var nbPoppingTexts = GameObject.FindGameObjectsWithTag(Constants.TagPoppingText);
        _text.sortingOrder = nbPoppingTexts.Length + 1;
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
        if (transform.position.y >= _positionToReach.y - 0.01f && Time.time >= _floatingTime)
        {
            //tag = Constants.TagCell;
            _text.color = Color.Lerp(_text.color, _colorToFade, 0.1f);
            if (_text.color.a <= 0.01)
            {
                _isMoving = false;
                Destroy(gameObject);
            }
        }
    }
}
