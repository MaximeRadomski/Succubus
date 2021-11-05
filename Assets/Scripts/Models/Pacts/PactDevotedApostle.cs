using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactDevotedApostle : Pact
{
    public PactDevotedApostle()
    {
        Id = 0;
        Name = PactsData.Pacts[Id];
        MaxFight = 2;
        Description = $"grants you {Highlight("+2 damage")} / cancels your ability to {Highlight("hold")}.";
        ShortDescription = "+2 damage / can't hold";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactFlatDamage += 2;
        Cache.PactCanHold = false;
    }
}
