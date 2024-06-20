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
        Cooldown = 16;
    }

    protected override void Effect()
    {
        Cache.RandomizedAttackType = AttackType.FromId(Random.Range(1, AttackType.AttackTypes.Count));
        ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).RandomizeOpponentAttack();
        base.Effect();
    }
}
