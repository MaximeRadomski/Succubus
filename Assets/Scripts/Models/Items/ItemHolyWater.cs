﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolyWater : Item
{
    public ItemHolyWater()
    {
        Id = 0;
        Name = ItemsData.Items[Id];
        Description = $"clears {Highlight("4 waste rows")}.";
        Rarity = Rarity.Common;
        Cooldown = 8;
    }

    protected override object Effect()
    {
        _gameplayControler.CheckForWasteRows(4);
        _gameplayControler.ClearLineSpace();
        return base.Effect();
    }
}
