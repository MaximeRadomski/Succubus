﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolyWater : Item
{
    public ItemHolyWater()
    {
        Id = 0;
        Name = ItemsData.Items[Id];
        Description = $"clears {Highlight("6 waste rows")}.";
        Rarity = Rarity.Common;
        Cooldown = 6;
    }

    protected override void Effect()
    {
        _gameplayControler.CheckForWasteRows(4);
        _gameplayControler.ClearLineSpace();
        base.Effect();
    }
}
