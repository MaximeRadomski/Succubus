using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooThermalGoggles : Tattoo
{
    public TattooThermalGoggles()
    {
        Id = 8;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        StatStr = null;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.AirPieceOpacity += Stat;
    }

    public override string GetDescription()
    {
        return $"you see air pieces {StatToString("+", "0%")} more distinctively";
    }
}
