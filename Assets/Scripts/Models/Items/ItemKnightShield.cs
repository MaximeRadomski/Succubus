using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKnightShield : Item
{
    public ItemKnightShield()
    {
        Id = 15;
        Name = ItemsData.Items[Id];
        Description = $"blocks {Highlight("1 attack")} every 3 attacks for the duration of the fight.";
        Rarity = Rarity.Rare;
        Cooldown = 14;
    }

    protected override object Effect()
    {
        Constants.BlockPerAttack = 0;
        return base.Effect();
    }
}
