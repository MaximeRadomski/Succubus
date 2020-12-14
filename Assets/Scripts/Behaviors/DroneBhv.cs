using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBhv : MonoBehaviour
{
    private GameplayControler _gameplayControler;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _targetSpriteRenderer;

    public void Init(GameplayControler gameplayControler, Realm realm)
    {
        _gameplayControler = gameplayControler;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Drone_" + realm.GetHashCode());
        _targetSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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

    public void OnPieceLocked(GameObject lockedCurrentPiece)
    {
        if (IsTargeted(lockedCurrentPiece))
        {
            _gameplayControler.AfterSpawn = null;
            ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).PlayHit();
            Destroy(gameObject);
        }
    }
}
