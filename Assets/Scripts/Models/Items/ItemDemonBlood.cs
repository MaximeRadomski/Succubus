using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDemonBlood : Item
{
    public ItemDemonBlood()
    {
        Id = 1;
        Name = "Demon Blood";
        Description = "Clears all dark rows";
        Rarity = Rarity.Common;
        Cooldown = 15;
    }

    public override bool Activate()
    {
        return base.Activate();
    }
}
