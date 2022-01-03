using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactInstantEpiphany : Pact
{
    public PactInstantEpiphany()
    {
        Id = 12;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"reduces your special's total cooldown by {Highlight("5")}.";
        Cons = $"reduces your attack by {Highlight("-2 damage")}.";
        ShortDescription = $"-5 special cooldown {Highlight("/")} -2 damage";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactSpecialTotalCooldownReducer += 5;
        Cache.PactFlatDamage -= 2;
        Cache.ResetSelectedCharacterSpecialCooldown(character);
    }
}
