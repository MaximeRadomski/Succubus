using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactShortRapture : Pact
{
    public PactShortRapture()
    {
        Id = 8;
        Name = PactsData.Pacts[Id];
        MaxFight = 1;
        Pros = $"grants you {Highlight("+15 damage")}.";
        Cons = $"triggers a {Highlight("+50% chance")} of an additional block on your pieces.";
        ShortDescription = $"+15 damage {Highlight("/")} +50% additional block";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactFlatDamage += 15;
        Cache.PactChanceAdditionalBlock += 50;
    }
}
