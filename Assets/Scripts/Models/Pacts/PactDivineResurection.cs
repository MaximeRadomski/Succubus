using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactDivineResurection : Pact
{
    public PactDivineResurection()
    {
        Id = 4;
        Name = PactsData.Pacts[Id];
        MaxFight = 3;
        Pros = $"{Highlight("resurrects")} you on death.";
        Cons = $"cancels the {Highlight("step's loot")}.";
        ShortDescription = $"resurrects {Highlight("/")} no loot";
        Rarity = Rarity.Rare;
    }

    public override void ApplyPact(Character character)
    {
        Cache.PactResurrection = true;
        Cache.PactNoLoot = true;
    }
}
