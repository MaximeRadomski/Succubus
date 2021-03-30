using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialHollowed : Special
{
    private int _nbPiece;

    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked || _gameplayControler.CurrentPiece.GetComponent<Piece>().HasBlocksAffectedByGravity)
            return false;
        if (!base.Activate())
            return false;
        _nbPiece = 1;
        OnNewPiece(_gameplayControler.CurrentPiece);
        return true;
    }

    public override void OnNewPiece(GameObject piece)
    {
        base.OnNewPiece(piece);
        if (_nbPiece <= 0)
            return;
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsHollowed = true;
        foreach (Transform child in _gameplayControler.CurrentPiece.transform)
        {
            _gameplayControler.Instantiator.NewGravitySquare(child.gameObject, _character.Realm.ToString());
        }
        MonoBehaviour.Destroy(_gameplayControler.CurrentGhost);
        _gameplayControler.DropGhost();
    }

    public override void OnPieceLocked(GameObject piece)
    {
        base.OnPieceLocked(piece);
        --_nbPiece;
    }
}
