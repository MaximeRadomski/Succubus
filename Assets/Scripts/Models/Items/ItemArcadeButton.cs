using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemArcadeButton : Item
{
    private float _delay = 4.0f;

    public ItemArcadeButton()
    {
        Id = 31;
        Name = ItemsData.Items[Id];
        Description = $"for the next {Highlight($"{Mathf.RoundToInt(_delay)} seconds")} after activation, each locked piece does {Highlight($"{Constants.ArcadeDamage} damage")}.";
        Rarity = Rarity.Common;
        Cooldown = 8;
    }

    protected override void Effect()
    {
        Cache.ArcadeTime = Time.time + _delay;
        base.Effect();
    }
}
