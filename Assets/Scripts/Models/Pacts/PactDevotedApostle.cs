using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactDevotedApostle : Pact
{
    public PactDevotedApostle()
    {
        Id = 0;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"grants you {Highlight("+10 damage")}.";
        Cons = $"cancels your ability to {Highlight("hold")}.";
        ShortDescription = $"+10 damage {Highlight("/")} can't hold";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactFlatDamage += 10;
        Cache.PactNoHold = true;
    }
}
