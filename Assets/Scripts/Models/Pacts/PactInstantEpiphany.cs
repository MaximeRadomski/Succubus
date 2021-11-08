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
        Pros = $"reduces your special's total cooldown by {Highlight("2")}.";
        Cons = $"reduces your attack by {Highlight("-2 damage")}.";
        ShortDescription = $"-2 special cooldown {Highlight("/")} -2 damage";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactSpecialTotalCooldownReducer += 2;
        Cache.PactFlatDamage -= 2;
    }
}
