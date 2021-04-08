using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWoodenCross : Item
{
    public ItemWoodenCross()
    {
        Id = 8;
        Name = ItemsData.Items[Id];
        Description = "resets the gravity strength of the room";
        Rarity = Rarity.Common;
        Cooldown = 10;
    }

    protected override object Effect()
    {
        _gameplayControler.SetGravity(1);
        return base.Effect();
    }
}
