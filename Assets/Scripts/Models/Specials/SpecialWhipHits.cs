using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialWhipHits : Special
{
    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        _gameplayControler.Bag = _gameplayControler.Bag.Insert(0, _gameplayControler.CurrentPiece.GetComponent<Piece>().Letter);
        Object.Destroy(_gameplayControler.CurrentPiece ?? null);
        Object.Destroy(_gameplayControler.CurrentGhost ?? null);
        _gameplayControler.Bag = _gameplayControler.Bag.Insert(0, "DDD");
        _gameplayControler.Spawn();
        return true;
    }
}
