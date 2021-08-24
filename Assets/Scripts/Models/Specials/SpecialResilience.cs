using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialResilience : Special
{
    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        Constants.IsNextOpponentAttackCanceled = true;
        return true;
    }
}
