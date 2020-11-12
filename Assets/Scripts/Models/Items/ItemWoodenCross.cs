using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWoodenCross : Item
{
    public ItemWoodenCross()
    {
        Id = 8;
        Name = ItemsData.Items[Id];
        Description = "reset the speed of the room";
        Rarity = Rarity.Rare;
        Cooldown = 10;
    }

    protected override object Effect()
    {
        _gameplayControler.SetGravity(1);
        return base.Effect();
    }
}
