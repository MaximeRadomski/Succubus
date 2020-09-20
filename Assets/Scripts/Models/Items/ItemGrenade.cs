using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrenade : Item
{
    public ItemGrenade()
    {
        Id = 2;
        Name = "Grenade";
        Description = "Clears your last four rows";
        Rarity = Rarity.Common;
        Cooldown = 15;
    }

    public override bool Activate()
    {
        if (!base.Activate())
            return false;
        _gameplayControler.SceneBhv.Paused = true;
        ClearFromTop(4);
        _gameplayControler.SceneBhv.Paused = false;
        return true;
    }

    public void ClearFromTop(int nbRows)
    {
        int start = _gameplayControler.GetHighestBlock();
        int end = start - (nbRows - 1);
        for (int y = start; y >= end; --y)
        {
            if (y < 0)
                break;
            _gameplayControler.DeleteLine(y);
        }
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
    }
}
