using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooMap : Tattoo
{
    public TattooMap()
    {
        Id = 52;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "all steps";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.MapAquired = true;
    }

    public override string GetDescription()
    {
        return $"reveals {StatToString()} of the current level.";
    }
}
