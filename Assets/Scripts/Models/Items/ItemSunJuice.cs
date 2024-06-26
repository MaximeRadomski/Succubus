﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSunJuice : Item
{
    public ItemSunJuice()
    {
        Id = 24;
        Name = ItemsData.Items[Id];
        Description = $"removes {Highlight("8 lines")} from the top.\n{Highlight("5 uses")}, no cooldown. replenishes when you ascend.";
        Rarity = Rarity.Legendary;
        Cooldown = -1;
        Type = ItemType.UsesBased;
        Uses = 5;
    }

    protected override void Effect()
    {
        int nbRows = 8;
        int start = _gameplayControler.GetHighestBlock();
        int end = start - (nbRows - 1);
        for (int y = start; y >= end; --y)
        {
            if (y < Cache.PlayFieldMinHeight)
                break;
            _gameplayControler.DeleteLine(y);
        }
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
        base.Effect();
    }
}
