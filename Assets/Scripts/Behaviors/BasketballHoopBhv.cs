using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballHoopBhv : FrameRateBehavior
{
    private GameplayControler _gameplayControler;
    private SpriteRenderer _targetSpriteRenderer;

    public void Init(GameplayControler gameplayControler)
    {
        _gameplayControler = gameplayControler;
        _targetSpriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    private void GetGameplayControler()
    {
        _gameplayControler = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>();
    }

    void Update()
    {
        if (_gameplayControler == null)
            GetGameplayControler();
        if (_gameplayControler == null || _gameplayControler.CurrentPiece == null)
            return;
        if (IsTargeted(_gameplayControler.CurrentPiece))
            _targetSpriteRenderer.enabled = true;
        else
            _targetSpriteRenderer.enabled = false;
    }

    private bool IsTargeted(GameObject piece)
    {
        var rangeX = piece.GetComponent<Piece>().GetRangeX();
        //return Vector2.Distance(new Vector2(piece.transform.GetChild(0).position.x - 0.5f + piece.GetComponent<Piece>().XFromSpawn, 0.0f), new Vector2(transform.position.x, 0.0f)) < 1.5f;
        var x = Mathf.RoundToInt(transform.position.x);
        return x >= rangeX[0] && x <= rangeX[1];
    }

    public void RandomizePosition()
    {
        var randomX = Random.Range(0, 9);
        transform.position = new Vector3(randomX, transform.position.y, 0.0f);
    }
}
