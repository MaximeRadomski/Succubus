using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBloodlustBlade : Item
{
    private int _times = 6;

    public ItemBloodlustBlade()
    {
        Id = 28;
        Name = ItemsData.Items[Id];
        Description = $"deals {Highlight($"{_times} times")} your damage but reduces your playfield height by {Highlight("2 lines")}.";
        Rarity = Rarity.Legendary;
        Cooldown = 2;
    }

    protected override object Effect()
    {
        _gameplayControler.SceneBhv.DamageOpponent(_times * _character.GetAttack(), _gameplayControler.CharacterInstanceBhv.gameObject, textRealm: Realm.Hell);
        _gameplayControler.ReducePlayHeight(2);
        return base.Effect();
    }
}
