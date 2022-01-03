using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactSinfulDivinity : Pact
{
    public PactSinfulDivinity()
    {
        Id = 17;
        Name = PactsData.Pacts[Id];
        MaxFight = 4;
        Pros = $"{Highlight("resets your playfield")} after a fight.";
        Cons = $"reduces your opponent's cooldown by {Highlight("-1 seconds")}.";
        ShortDescription = $"resets playfield {Highlight("/")} -1s opponent's cooldown";
        Rarity = Rarity.Rare;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactNoLastFightPlayField = true;
        Cache.PactEnemyMaxCooldownMalus -= 1;
    }
}
