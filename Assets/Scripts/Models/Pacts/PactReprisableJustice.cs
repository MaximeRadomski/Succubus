using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactReprisableJustice : Pact
{
    public PactReprisableJustice()
    {
        Id = 19;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"prevents {Highlight("haste attacks")} for 2 fights.";
        Cons = $"only {Highlight("haste attacks")} for 1 fight.";
        ShortDescription = $"prevents haste {Highlight("/")} only haste";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        if (NbFight < 2)
            Cache.PactNoHaste = true;
        else
            Cache.PactOnlyHaste = true;
    }
}
