using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkyDown : Special
{
    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        //_gameplayControler.StartCoroutine(Test());
        var oldOnHoldValue = _gameplayControler.GameplayOnHold;
        _gameplayControler.GameplayOnHold = true;
        var takeBackY = -1;
        for (int y = Cache.HeightLimiter; y < Constants.PlayFieldHeight; ++y)
        {
            for (int x = 0; x < Constants.PlayFieldWidth; ++x)
            {
                if (y > Cache.HeightLimiter && _gameplayControler.PlayFieldBhv.Grid[x, y] != null && _gameplayControler.PlayFieldBhv.Grid[x, y - 1] == null)
                {
                    _gameplayControler.PlayFieldBhv.Grid[x, y - 1] = _gameplayControler.PlayFieldBhv.Grid[x, y];
                    _gameplayControler.PlayFieldBhv.Grid[x, y] = null;
                    _gameplayControler.PlayFieldBhv.Grid[x, y - 1].transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                    if (takeBackY == -1)
                        takeBackY = y;
                    --x;
                    --y;
                }
                else if (takeBackY != -1)
                {
                    y = takeBackY;
                    takeBackY = -1;
                }
            }
        }
        _gameplayControler.GameplayOnHold = oldOnHoldValue;
        return true;
    }

    //private IEnumerator Test()
    //{
        
    //}
}
