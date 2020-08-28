using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicGameSceneBhv : GameSceneBhv
{
    void Start()
    {
        Init();
        _gameplayControler.GetComponent<GameplayControler>().StartGameplay(_currentOpponent.GravityLevel, Realm.Hell, Realm.Hell);
    }

    override public void OnGameOver()
    {
        
    }

    public override void OnNewPiece()
    {
        if (_gameplayControler == null)
            return;
    }

    public override void OnPieceLocked(string pieceLetter)
    {
        base.OnPieceLocked(pieceLetter);
    }

    public override void OnLinesCleared(int nbLines, bool isB2B)
    {
        base.OnLinesCleared(nbLines, isB2B);
    }

    public override void OnCombo(int nbCombo)
    {
        base.OnCombo(nbCombo);
    }

    public override void OnPerfectClear()
    {
        base.OnPerfectClear();
    }
}
