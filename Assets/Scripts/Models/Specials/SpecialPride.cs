using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPride : Special
{
    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        int nbRows = Random.Range(2, 3);
        int tX = Random.Range(1, 8);
        int start = _gameplayControler.GetHighestBlock();
        if (start + Cache.HeightLimiter < nbRows + Cache.HeightLimiter - 1)
            start = nbRows + Cache.HeightLimiter - 1;
        int end = start - (nbRows - 1);
        for (int y = start; y >= end; --y)
        {
            if (y < Cache.HeightLimiter)
                break;
            _gameplayControler.DeleteLine(y);
            _gameplayControler.FillLine(y, AttackType.WasteRow, _character.Realm);
        }
        switch (nbRows)
        {
            case 2:
                TwoLines(tX, start);
                break;
            case 3:
                ThreeLines(tX, start);
                break;
        }
        while (!_gameplayControler.IsPiecePosValid(_gameplayControler.CurrentPiece))
            _gameplayControler.CurrentPiece.transform.position += new Vector3(0.0f, 1.0f, 0.0f);
        for (int forcedY = 2; forcedY > 0; --forcedY)
        {
            _gameplayControler.CurrentPiece.transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            if (!_gameplayControler.IsPiecePosValid(_gameplayControler.CurrentPiece))
            {
                _gameplayControler.CurrentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                break;
            }
        }
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
        return true;
    }

    private void TwoLines(int tX, int yStart)
    {
        var blockPiece = _gameplayControler.Instantiator.NewPiece(AttackType.WasteRow.ToString(), _character.Realm.ToString(), new Vector3(tX + (Random.Range(0, 1) == 0 ? -1 : +1), yStart + 1, 0.0f));
        _gameplayControler.AddToPlayField(blockPiece);
        DestroyBlock(tX - 1, yStart);
        DestroyBlock(tX, yStart);
        DestroyBlock(tX + 1, yStart);
        DestroyBlock(tX, yStart - 1);
    }

    private void ThreeLines(int tX, int yStart)
    {
        int leftRight = tX <= 4 ? -1 : +1;
        var blockPiece = _gameplayControler.Instantiator.NewPiece(AttackType.WasteRow.ToString(), _character.Realm.ToString(), new Vector3(tX, yStart + 2, 0.0f));
        _gameplayControler.AddToPlayField(blockPiece);
        blockPiece = _gameplayControler.Instantiator.NewPiece(AttackType.WasteRow.ToString(), _character.Realm.ToString(), new Vector3(tX + leftRight, yStart + 1, 0.0f));
        _gameplayControler.AddToPlayField(blockPiece);
        DestroyBlock(tX, yStart);
        DestroyBlock(tX, yStart - 1);
        DestroyBlock(tX - leftRight, yStart - 1);
        DestroyBlock(tX, yStart - 2);
    }

    private void DestroyBlock(int roundedX, int roundedY)
    {
        if (roundedX < 0 || roundedX >= Constants.PlayFieldWidth
            || roundedY < Cache.HeightLimiter || roundedY >= Constants.PlayFieldHeight
            || _gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY]?.GetComponent<BlockBhv>()?.Indestructible == true)
            return;
        _gameplayControler.Instantiator.NewFadeBlock(Realm.Earth, new Vector3(roundedX, roundedY, 0.0f), 5, 0);
        if (_gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY] != null)
        {
            MonoBehaviour.Destroy(_gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY].gameObject);
            _gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY] = null;
        }
    }
}
