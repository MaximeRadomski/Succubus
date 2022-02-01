using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDarkMilk : Item
{
    public ItemDarkMilk()
    {
        Id = 26;
        Name = ItemsData.Items[Id];
        Description = $"change your opponent realm to your {Highlight("weaker")} one.";
        Rarity = Rarity.Common;
        Cooldown = 13;
    }

    protected override void Effect()
    {
        Cache.CurrentOpponentChangedRealm = Helper.GetInferiorFrom(_character.Realm);
        ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).AlterOpponentRealm(Cache.CurrentOpponentChangedRealm);
        ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).OpponentInstanceBhv.Malus(Cache.CurrentOpponentChangedRealm, 2.0f);
        base.Effect();
    }
}
