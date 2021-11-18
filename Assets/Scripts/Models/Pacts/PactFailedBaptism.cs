using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactFailedBaptism : Pact
{
    public PactFailedBaptism()
    {
        Id = 1;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"makes you {Highlight("holy")}.";
        Cons = $"triggers a {Highlight("+5% chance")} of an additional block on your pieces.";
        ShortDescription = $"holiness {Highlight("/")} +5% additional block";
        Rarity = Rarity.Rare;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactCharacterRealm = Realm.Heaven;
        Cache.PactChanceAdditionalBlock += 5;
    }
}
