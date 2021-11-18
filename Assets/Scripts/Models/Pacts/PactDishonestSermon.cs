using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactDishonestSermon : Pact
{
    public PactDishonestSermon()
    {
        Id = 14;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"gives your opponent {Highlight("+6 seconds")} cooldown.";
        Cons = $"cancels the {Highlight("step's loot")}.";
        ShortDescription = $"+6s opponent's cooldown {Highlight("/")} no loot";
        Rarity = Rarity.Rare;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactEnemyMaxCooldownMalus += 6;
        Cache.PactNoLoot = true;
    }
}
