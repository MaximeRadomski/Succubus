using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlippingCoin : Item
{
    public ItemFlippingCoin()
    {
        Id = 11;
        Name = ItemsData.Items[Id];
        Description = $"for the duration of the fight, your attacks and your opponent's ones only have a {Highlight("50% chance")} of happening.";
        Rarity = Rarity.Common;
        Cooldown = 10;
    }

    protected override void Effect()
    {
        Cache.ChanceAttacksHappeningPercent = 50;
        base.Effect();
    }
}
