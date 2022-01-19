using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVoodooDoll : Item
{
    public ItemVoodooDoll()
    {
        Id = 3;
        Name = ItemsData.Items[Id];
        Description = $"deals {Highlight("15 damage")}, but bypasses your current opponent's {Highlight("cooldown")} and activates its attack.";
        Rarity = Rarity.Common;
        Cooldown = 1;
    }

    protected override object Effect()
    {
        _gameplayControler.SceneBhv.DamageOpponent(15, _gameplayControler.CharacterInstanceBhv.gameObject, textRealm: Realm.Earth);
        ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).OpponentAttackIncoming();
        return base.Effect();
    }
}
