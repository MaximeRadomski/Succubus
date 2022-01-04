using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPunishment : Special
{
    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        int left = 0;
        int right = 0;
        for (int x = 0; x < Constants.PlayFieldWidth; ++x)
        {
            for (int y = _gameplayControler.GetHighestBlockOnX(x); y >= Cache.HeightLimiter; --y)
            {
                if (_gameplayControler.PlayFieldBhv.Grid[x, y] == null)
                {
                    if (x <= 4)
                        ++left;
                    else
                        ++right;
                }
            }
        }

        int start;
        int end;
        if (left > right)
        {
            start = 0;
            end = 4;
        }
        else
        {
            start = 5;
            end = 9;
        }
        for (int i = start; i <= end; ++i)
            _gameplayControler.DeleteColumn(i);
        return true;
    }
}
