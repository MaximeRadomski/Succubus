using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVoodooDoll : Item
{
    public ItemVoodooDoll()
    {
        Id = 3;
        Name = ItemsData.Items[Id];
        Description = "bypasses your current opponent's cooldown and activates its attack";
        Rarity = Rarity.Common;
        Cooldown = 1;
    }

    public override bool Activate(Character character, GameplayControler gameplayControler)
    {
        if (!base.Activate(character, gameplayControler))
            return false;
        Constants.CurrentOpponentCooldown = 1000;
        return true;
    }
}
