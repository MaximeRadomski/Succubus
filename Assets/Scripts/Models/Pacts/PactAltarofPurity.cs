using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactAltarofPurity : Pact
{
    public PactAltarofPurity()
    {
        Id = 13;
        Name = PactsData.Pacts[Id];
        MaxFight = 4;
        Pros = $"grants you {Highlight("stealth")} until your first attack.";
        Cons = $"reduces your attack by {Highlight("-2 damage")}.";
        ShortDescription = $"stealth {Highlight("/")} -2 damage";
        Rarity = Rarity.Legendary;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactStealth = true;
        Cache.PactFlatDamage -= 2;
    }
}
