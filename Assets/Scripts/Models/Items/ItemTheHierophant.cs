using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTheHierophant : Item
{
    public ItemTheHierophant()
    {
        Id = 20;
        Name = ItemsData.Items[Id];
        Description = $"prevents the {Highlight("lock delay")} to be {Highlight("shortened")}, and when a piece isn't locked by a hard drop, it drops again.";
        Rarity = Rarity.Rare;
        Cooldown = 16;
    }

    protected override void Effect()
    {
        Cache.BonusLockDelay = 0.5f;
        _gameplayControler.SetLockDelay();
        base.Effect();
    }
}
