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
        Description = $"makes you {Highlight("holy")} / triggers a {Highlight("+10% chance")} of an additional block on your pieces.\nduration {Highlight($"{MaxFight} fights")}.";
        ShortDescription = "holiness / additional block chance";
        Rarity = Rarity.Rare;
    }

    public override void ApplyPact()
    {
        Cache.PactCharacterRealm = Realm.Heaven;
        Cache.PactChanceAdditionalBlock += 10;
    }
}
