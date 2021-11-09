using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactPleasantTribulation : Pact
{
    public PactPleasantTribulation()
    {
        Id = 15;
        Name = PactsData.Pacts[Id];
        MaxFight = 2;
        Pros = $"gives your opponent {Highlight("+2 seconds")} cooldown.";
        Cons = $"cancels {Highlight("soft drops")}.";
        ShortDescription = $"+2s opponent's cooldown {Highlight("/")} cancels soft drops";
        Rarity = Rarity.Common;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactEnemyMaxCooldownMalus += 2;
        Cache.PactNoSoftDrop = true;
    }
}
