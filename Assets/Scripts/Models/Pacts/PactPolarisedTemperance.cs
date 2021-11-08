using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactPolarisedTemperance : Pact
{
    public PactPolarisedTemperance()
    {
        Id = 6;
        Name = PactsData.Pacts[Id];
        MaxFight = 6;
        Pros = $"grants you {Highlight("+2 damage")}.";
        Cons = $"cancels your probability of {Highlight("critical hits")}.";
        ShortDescription = $"+2 damage {Highlight("/")} no critical hits";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactFlatDamage += 2;
        Cache.PactNoCrit = true;
    }
}
