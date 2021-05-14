using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTheHierophant : Item
{
    public ItemTheHierophant()
    {
        Id = 20;
        Name = ItemsData.Items[Id];
        Description = $"lock time gets longer by {Highlight("1 second")} for the duration of fight.";
        Rarity = Rarity.Rare;
        Cooldown = 8;
    }

    protected override object Effect()
    {
        Constants.BonusLockDelay += 1.0f;
        _gameplayControler.SetLockDelay();
        return base.Effect();
    }
}
