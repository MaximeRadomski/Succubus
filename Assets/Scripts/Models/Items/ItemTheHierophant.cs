using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTheHierophant : Item
{
    public ItemTheHierophant()
    {
        Id = 20;
        Name = ItemsData.Items[Id];
        Description = $"lock time gets longer by {Highlight("2 seconds")} for the duration of fight.";
        Rarity = Rarity.Rare;
        Cooldown = 8;
    }

    protected override object Effect()
    {
        Cache.BonusLockDelay += 2.0f;
        _gameplayControler.SetLockDelay();
        return base.Effect();
    }
}
