using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactHellishIdolatry : Pact
{
    public PactHellishIdolatry()
    {
        Id = 2;
        Name = PactsData.Pacts[Id];
        MaxFight = 4;
        Pros = $"halves your {Highlight("item's cooldown")}.";
        Cons = $"doubles your {Highlight("special's cooldown")}."; ;
        ShortDescription = $"item's cooldown ~2 {Highlight("/")} special's cooldown *2";
        Rarity = Rarity.Legendary;
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
