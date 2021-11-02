using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLuckyLadybug : Item
{
    public ItemLuckyLadybug()
    {
        Id = 14;
        Name = ItemsData.Items[Id];
        Description = $"gives you {Highlight("+33.33333333...%")} dodging chance for the rest of the fight.";
        Rarity = Rarity.Rare;
        Cooldown = 14;
    }

    protected override object Effect()
    {
        Cache.AddedDodgeChancePercent += 33;
        return base.Effect();
    }
}
