using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactShortParadise : Pact
{
    public PactShortParadise()
    {
        Id = 3;
        Name = PactsData.Pacts[Id];
        MaxFight = 1;
        Description = $"cancels gravity / cancels soft drops";
        ShortDescription = "cancels gravity / cancels soft drops";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactCooldownSwap = true;
        var tmpCurrentItem = PlayerPrefsHelper.GetCurrentItem();
        var newMaxItemCooldown = (tmpCurrentItem.Cooldown - tmpCurrentItem.Cooldown - character.ItemMaxCooldownReducer) / 2;
        if (Cache.CurrentItemCooldown > newMaxItemCooldown)
            Cache.CurrentItemCooldown = newMaxItemCooldown;
    }
}
