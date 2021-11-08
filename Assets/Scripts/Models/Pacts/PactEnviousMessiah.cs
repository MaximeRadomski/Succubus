using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactEnviousMessiah : Pact
{
    public PactEnviousMessiah()
    {
        Id = 11;
        Name = PactsData.Pacts[Id];
        MaxFight = 2;
        Pros = $"grants {Highlight("+5 cumulative damage")} to each combo.";
        Cons = $"reduces your attack by {Highlight("-5 damage")}.";
        ShortDescription = $"+5 combo damage {Highlight("/")} -5 damage";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactComboDamage += 5;
        Cache.PactFlatDamage -= 5;
    }
}
