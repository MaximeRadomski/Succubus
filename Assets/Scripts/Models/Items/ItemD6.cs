using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemD6 : Item
{
    public ItemD6()
    {
        Id = 12;
        Name = ItemsData.Items[Id];
        Description = $"{Highlight("randomizes")} your opponent attack type to another one.";
        Rarity = Rarity.Common;
        Cooldown = 8;
    }

    protected override object Effect()
    {
        Cache.RandomizedAttackType = (AttackType)Random.Range(1, Helper.EnumCount<AttackType>());
        ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).RandomizeOpponentAttack();
        return base.Effect();
    }
}
