using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUncertainty : Special
{
    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        var nbRows = Random.Range(1, 11);
        int start = Cache.HeightLimiter;
        int end = start + (nbRows - 1);
        for (int y = start; y <= end; ++y)
            _gameplayControler.DeleteLine(y);
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
        return true;
    }
}
