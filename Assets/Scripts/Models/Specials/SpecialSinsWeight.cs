using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSinsWeight : Special
{
    private int _nbPiece;

    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        _nbPiece = 3;
        //_gameplayControler.Bag = _gameplayControler.Bag.Insert(0, _gameplayControler.CurrentPiece.GetComponent<Piece>().Letter);
        //Object.Destroy(_gameplayControler.CurrentPiece ?? null);
        //Object.Destroy(_gameplayControler.CurrentGhost ?? null);
        //_gameplayControler.Spawn();
        OnNewPiece(_gameplayControler.CurrentPiece);
        return true;
    }

    public override void OnNewPiece(GameObject piece)
    {
        base.OnNewPiece(piece);
        if (_nbPiece <= 0)
            return;
        _gameplayControler.CurrentPiece.GetComponent<Piece>().AlterBlocksAffectedByGravity(true);
        foreach (Transform child in _gameplayControler.CurrentPiece.transform)
        {
            _gameplayControler.Instantiator.NewGravitySquare(child.gameObject, _character.Realm.ToString());
        }
        _gameplayControler.DropGhost();
    }

    public override void OnPieceLocked(GameObject piece)
    {
        base.OnPieceLocked(piece);
        --_nbPiece;
    }
}
