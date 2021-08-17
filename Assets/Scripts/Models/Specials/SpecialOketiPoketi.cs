using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialOketiPoketi : Special
{
    private int _nbPiece;

    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        _nbPiece = 5;
        OnNewPiece(_gameplayControler.CurrentPiece);
        return true;
    }

    public override void OnNewPiece(GameObject piece)
    {
        base.OnNewPiece(piece);
        if (_nbPiece <= 0)
            return;
        MonoBehaviour.Destroy(_gameplayControler.CurrentPiece.transform.GetChild(_gameplayControler.CurrentPiece.transform.childCount - 1).gameObject);
        MonoBehaviour.Destroy(_gameplayControler.CurrentGhost.transform.GetChild(_gameplayControler.CurrentGhost.transform.childCount - 1).gameObject);
        for (int i = 0; i < _nbPiece && i < 5; ++i)
            MonoBehaviour.Destroy(_gameplayControler.NextPieces[i].transform.GetChild(0).GetChild(_gameplayControler.NextPieces[i].transform.GetChild(0).childCount - 1).gameObject);
        _gameplayControler.DropGhost();
    }

    public override void OnPieceLocked(GameObject piece)
    {
        base.OnPieceLocked(piece);
        --_nbPiece;
    }
}
