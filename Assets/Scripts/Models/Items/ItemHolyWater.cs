using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolyWater : Item
{
    public override void Init(Character character, GameplayControler gameplayControler)
    {
        base.Init(character, gameplayControler);
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
