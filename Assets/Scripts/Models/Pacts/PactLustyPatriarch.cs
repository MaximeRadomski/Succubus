using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactLustyPatriarch : Pact
{
    public PactLustyPatriarch()
    {
        Id = 10;
        Name = PactsData.Pacts[Id];
        MaxFight = 4;
        Pros = $"grants you {Highlight("+75% crit chance")}.";
        Cons = $"cancels your ability to {Highlight("hold")}.";
        ShortDescription = $"+75% crit chance {Highlight("/")} can't hold";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactCritChance += 75;
        Cache.PactNoHold = true;
    }
}
