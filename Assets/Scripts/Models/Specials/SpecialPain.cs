using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPain : Special
{
    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        (this._gameplayControler.SceneBhv as ClassicGameSceneBhv).DamageOpponent(Random.Range(10, 101), this._gameplayControler.CharacterInstanceBhv.gameObject);
        return true;
    }
}
