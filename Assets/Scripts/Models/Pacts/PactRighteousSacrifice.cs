using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactRighteousSacrifice : Pact
{
    public PactRighteousSacrifice()
    {
        Id = 7;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"grants you {Highlight("+50% crit chance")}.";
        Cons = $"reduces your attack by {Highlight("-5 damage")}.";
        ShortDescription = $"+50% crit chance {Highlight("/")} -5 damage";
        Rarity = Rarity.Rare;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactCritChance += 50;
        Cache.PactFlatDamage -= 5;
    }
}
