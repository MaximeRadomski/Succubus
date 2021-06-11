using UnityEngine;
using System.Collections;

public class ItemReverseCrucifix : Item
{
    public ItemReverseCrucifix()
    {
        Id = 7;
        Name = ItemsData.Items[Id];
        Description = $"inflicts {Highlight("666 damage")} to your opponent (66 to bosses).";
        Rarity = Rarity.Legendary;
        Cooldown = 25;
    }

    protected override object Effect()
    {
        int damage = 666;
        if (_gameplayControler.SceneBhv.CurrentOpponent.Type == OpponentType.Boss)
            damage = 66;
        _gameplayControler.SceneBhv.DamageOpponent(damage, _gameplayControler.CharacterInstanceBhv.gameObject, textRealm: Realm.Hell);
        return base.Effect();
    }
}