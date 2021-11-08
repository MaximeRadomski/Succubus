using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactSmallParadise : Pact
{
    public PactSmallParadise()
    {
        Id = 3;
        Name = PactsData.Pacts[Id];
        MaxFight = 2;
        Pros = $"cancels {Highlight("gravity")}.";
        Cons = $"cancels {Highlight("soft drops")}.";
        ShortDescription = $"cancels gravity {Highlight("/")} cancels soft drops";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactZeroGravityOrSoftDrop = true;
    }
}
