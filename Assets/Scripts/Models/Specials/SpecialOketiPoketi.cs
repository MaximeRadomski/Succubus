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
        _nbPiece = 7;
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
        for (int i = 0; i < _nbPiece - 1 && i < 5; ++i)
        {
            var iteratedNextPieceContainerLastChildId = _gameplayControler.NextPieces[i].transform.childCount - 1;
            var iteratedNextPieceChildCount = _gameplayControler.NextPieces[i].transform.GetChild(iteratedNextPieceContainerLastChildId).childCount;
            MonoBehaviour.Destroy(_gameplayControler.NextPieces[i].transform.GetChild(iteratedNextPieceContainerLastChildId).GetChild(iteratedNextPieceChildCount - 1).gameObject);
        }
        _gameplayControler.DropGhost();
    }

    public override void OnPieceLocked(GameObject piece)
    {
        base.OnPieceLocked(piece);
        --_nbPiece;
    }
}
