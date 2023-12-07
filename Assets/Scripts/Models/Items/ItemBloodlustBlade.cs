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
        Description = $"deals {Highlight($"{_times} times")} your attack but reduces your playfield height by {Highlight("1 lines")}.";
        Rarity = Rarity.Legendary;
        Cooldown = 2;
    }

    protected override void Effect()
    {
        _gameplayControler.SceneBhv.DamageOpponent(_times * _character.GetAttack(), _gameplayControler.CharacterInstanceBhv.gameObject, textRealm: Realm.Hell);
        _gameplayControler.ShrinkPlayHeight(1);
        base.Effect();
    }
}
