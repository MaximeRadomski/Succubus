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
        Pros = $"{Highlight("+12 damage")} for 2 fights.";
        Cons = $"{Highlight("-6 damage")} for 1 fight.";
        ShortDescription = $"+12 damage {Highlight("/")} -6 damage";
        Rarity = Rarity.Legendary;
    }

    public override void ApplyPact(Character character)
    {
        if (NbFight < 2)
            Cache.PactFlatDamage += 12;
        else
            Cache.PactFlatDamage -= 6;
    }
}
