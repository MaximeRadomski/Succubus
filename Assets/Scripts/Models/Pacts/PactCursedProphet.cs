using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactCursedProphet : Pact
{
    public PactCursedProphet()
    {
        Id = 9;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"makes you {Highlight("human")}.";
        Cons = $"triggers a {Highlight("+5% chance")} of an additional block on your pieces.";
        ShortDescription = $"humanity {Highlight("/")} +5% additional block";
        Rarity = Rarity.Rare;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactCharacterRealm = Realm.Earth;
        Cache.PactChanceAdditionalBlock += 5;
    }
}
