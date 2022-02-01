using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMoldFashioned : Item
{
    public ItemMoldFashioned()
    {
        Id = 13;
        Name = ItemsData.Items[Id];
        Description = $"{Highlight("halves")} your opponent remaining health, but also {Highlight("halves")} its max cooldown.";
        Rarity = Rarity.Rare;
        Cooldown = 14;
    }

    protected override void Effect()
    {
        Cache.HalvedCooldown = true;
        _gameplayControler.SceneBhv.DamageOpponent(Cache.CurrentOpponentHp / 2, null, textRealm: Realm.Heaven);
        ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).HalveOpponentMaxCooldown();
        base.Effect();
    }
}
