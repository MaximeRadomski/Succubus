using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWoodenCross : Item
{
    public ItemWoodenCross()
    {
        Id = 8;
        Name = ItemsData.Items[Id];
        Description = $"resets the {Highlight("gravity level")} of the room.";
        Rarity = Rarity.Common;
        Cooldown = 10;
    }

    protected override void Effect()
    {
        _gameplayControler.SetGravity(1, shoudlBeAlteredByTattoos: false);
        base.Effect();
    }
}
