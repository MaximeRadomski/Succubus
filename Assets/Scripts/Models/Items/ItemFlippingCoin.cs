﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlippingCoin : Item
{
    public ItemFlippingCoin()
    {
        Id = 11;
        Name = ItemsData.Items[Id];
        Description = "for the duration of the fight, your attacks and your opponent's ones only have a 50% chance of happening";
        Rarity = Rarity.Common;
        Cooldown = 8;
    }

    protected override object Effect()
    {
        _gameplayControler.SetGravity(1);
        return base.Effect();
    }
}