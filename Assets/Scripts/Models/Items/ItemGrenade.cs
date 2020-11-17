using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrenade : Item
{
    public ItemGrenade()
    {
        Id = 2;
        Name = ItemsData.Items[Id];
        Description = "clears your last four rows";
        Rarity = Rarity.Common;
        Cooldown = 12;
    }

    protected override object Effect()
    {
        int nbRows = 4;
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
        return base.Effect();
    }
}
