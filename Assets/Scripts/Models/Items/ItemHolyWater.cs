using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolyWater : Item
{
    public ItemHolyWater()
    {
        Id = 0;
        Name = "Holy Water";
        Description = "clears all waste rows";
        Rarity = Rarity.Common;
        Cooldown = 15;
    }

    public override bool Activate()
    {
        return base.Activate();
    }
}
