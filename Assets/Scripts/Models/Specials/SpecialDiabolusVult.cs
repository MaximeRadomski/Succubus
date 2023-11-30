using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialDiabolusVult : Special
{
    private int _nbRows;

    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        _nbRows = 4;
        int start = _gameplayControler.GetHighestBlock();
        int end = start - (_nbRows - 1);
        for (int y = start; y >= end; --y)
        {
            if (y < Cache.PlayFieldMinHeight)
                break;
            _gameplayControler.DeleteLine(y);
        }
        var minY = _gameplayControler.IncreaseAllAboveLines(_nbRows);
        var nbHole = 1;
        int emptyStart = UnityEngine.Random.Range(0, 10 + 1 - nbHole);
        int emptyEnd = emptyStart + nbHole - 1;
        for (int y = minY; y < minY + _nbRows; ++y)
        {
            _gameplayControler.FillLine(y, AttackType.WasteRow, _character.Realm, emptyStart, emptyEnd);
        }
        bool hasToMovePieceUp = false;
        while (!_gameplayControler.IsPiecePosValid(_gameplayControler.CurrentPiece))
        {
            _gameplayControler.CurrentPiece.transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            hasToMovePieceUp = true;
        }
        if (hasToMovePieceUp)
            _gameplayControler.ResetLock();
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
        return true; 
    }

    public override void OnPerfectClear()
    {
        Debug.Log("Perfect Clear can't reset Diabolus Vult");
    }
}
