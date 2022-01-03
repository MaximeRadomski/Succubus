using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactFalseTestimony : Pact
{
    public PactFalseTestimony()
    {
        Id = 5;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"{Highlight("+15 damage")} for 2 fights.";
        Cons = $"{Highlight("-5 damage")} for 1 fight.";
        ShortDescription = $"+15 damage {Highlight("/")} -5 damage";
        Rarity = Rarity.Legendary;
    }

    public override void ApplyPact(Character character)
    {
        if (NbFight < 2)
            Cache.PactFlatDamage += 15;
        else
            Cache.PactFlatDamage -= 5;
    }
}
