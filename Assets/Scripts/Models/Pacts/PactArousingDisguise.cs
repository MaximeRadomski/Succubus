using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactArousingDisguise : Pact
{
    public PactArousingDisguise()
    {
        Id = 18;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"prevents opponents {Highlight("haste attacks")}.";
        Cons = $"cancels your probability of {Highlight("critical hits")}.";
        ShortDescription = $"prevents haste {Highlight("/")} no critical hits";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactNoHaste = true;
        Cache.PactNoCrit = true;
    }
}
