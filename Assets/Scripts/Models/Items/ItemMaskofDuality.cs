using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMaskofDuality : Item
{
    public ItemMaskofDuality()
    {
        Id = 22;
        Name = ItemsData.Items[Id];
        Description = $"either deals {Highlight("4 times ")} your damage, or attacks you with {Highlight("4 garbage lines")}.";
        Rarity = Rarity.Common;
        Cooldown = 6;
    }

    protected override object Effect()
    {
        var random = Random.Range(0, 2);
        if (random == 0)
            _gameplayControler.SceneBhv.DamageOpponent(4 * _character.GetAttack(), _gameplayControler.CharacterInstanceBhv.gameObject, textRealm: Realm.Heaven);
        else
        {
            _gameplayControler.AttackWasteRows(_gameplayControler.CharacterInstanceBhv.gameObject, 4, Realm.Hell, 1, fromPlayer: true);
            _gameplayControler.DropGhost();
        }
        return base.Effect();
    }
}
