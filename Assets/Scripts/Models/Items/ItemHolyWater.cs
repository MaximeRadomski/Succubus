using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolyWater : Item
{
    public ItemHolyWater()
    {
        Id = 0;
        Name = "Holy Water";
        Description = "clear all waste rows";
        Rarity = Rarity.Common;
    }

    public override bool Activate()
    {
        return base.Activate();
    }
}
