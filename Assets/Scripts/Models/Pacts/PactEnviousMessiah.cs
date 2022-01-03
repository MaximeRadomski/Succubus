using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactEnviousMessiah : Pact
{
    public PactEnviousMessiah()
    {
        Id = 11;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"grants {Highlight("+6 cumulative damage")} to each combo.";
        Cons = $"reduces your attack by {Highlight("-2 damage")}.";
        ShortDescription = $"+6 combo damage {Highlight("/")} -2 damage";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactComboDamage += 6;
        Cache.PactFlatDamage -= 2;
    }
}
