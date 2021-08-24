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
            if (y < Constants.HeightLimiter)
                break;
            _gameplayControler.DeleteLine(y);
        }
        _gameplayControler.IncreaseAllAboveLines(_nbRows);
        var nbHole = 1;
        int emptyStart = UnityEngine.Random.Range(0, 10 + 1 - nbHole);
        int emptyEnd = emptyStart + nbHole - 1;
        for (int y = Constants.HeightLimiter; y < Constants.HeightLimiter + _nbRows; ++y)
        {
            _gameplayControler.FillLine(y, AttackType.WasteRow, _character.Realm, emptyStart, emptyEnd);
        }
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
        return true;
    }

    public override void OnPerfectClear()
    {
        Debug.Log("Perfect Clear can't reset Diabolus Vult");
    }
}
