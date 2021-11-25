using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactUnexpectedRevelation : Pact
{
    public PactUnexpectedRevelation()
    {
        Id = 16;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"reduces your special's total cooldown by {Highlight("5")}.";
        Cons = $"reduces your opponent's cooldown by {Highlight("-2 seconds")}.";
        ShortDescription = $"-5 special cooldown {Highlight("/")} -2s opponent's cooldown";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactSpecialTotalCooldownReducer += 5;
        Cache.PactEnemyMaxCooldownMalus -= 2;
        Cache.ResetSelectedCharacterSpecialCooldown(character);
    }
}
