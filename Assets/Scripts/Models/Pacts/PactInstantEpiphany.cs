using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactInstantEpiphany : Pact
{
    public PactInstantEpiphany()
    {
        Id = 12;
        Name = PactsData.Pacts[Id];
        MaxFight = 1;
        Pros = $"reduces your special's total cooldown by {Highlight("3")}.";
        Cons = $"reduces your attack by {Highlight("-3 damage")}.";
        ShortDescription = $"-3 special cooldown {Highlight("/")} -3 damage";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactSpecialTotalCooldownReducer += 3;
        Cache.PactFlatDamage -= 3;
        Cache.ResetSelectedCharacterSpecialCooldown(character);
    }
}
