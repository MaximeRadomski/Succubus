using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEliacube : Item
{
    public ItemEliacube()
    {
        Id = 21;
        Name = ItemsData.Items[Id];
        Description = $"resets your opponent {Highlight("cooldown")}.";
        Rarity = Rarity.Common;
        Cooldown = 5;
    }

    protected override object Effect()
    {
        ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).RestartOpponentCooldown();
        return base.Effect();
    }
}
