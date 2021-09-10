using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooFarm : Tattoo
{
    public TattooFarm()
    {
        Id = 57;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.ResourceFarmBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"resources you get on a step are increased by {StatToString("+")}.";
    }
}
