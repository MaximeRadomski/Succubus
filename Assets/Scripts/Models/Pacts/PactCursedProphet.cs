using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactCursedProphet : Pact
{
    public PactCursedProphet()
    {
        Id = 9;
        Name = PactsData.Pacts[Id];
        MaxFight = 5;
        Pros = $"makes you {Highlight("human")}.";
        Cons = $"triggers a {Highlight("+2% chance")} of an additional block on your pieces.";
        ShortDescription = $"humanity {Highlight("/")} +2% additional block";
        Rarity = Rarity.Rare;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactCharacterRealm = Realm.Earth;
        Cache.PactChanceAdditionalBlock += 2;
    }
}
