using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVoodooDoll : Item
{
    public ItemVoodooDoll()
    {
        Id = 3;
        Name = ItemsData.Items[Id];
        Description = $"{Highlight("bypasses")} your current opponent's {Highlight("cooldown")} and activates its attack.";
        Rarity = Rarity.Common;
        Cooldown = 1;
    }

    protected override object Effect()
    {
        Constants.CurrentOpponentCooldown = 1000;
        return base.Effect();
    }
}
