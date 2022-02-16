using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooThermalGoggles : Tattoo
{
    private float _multiplier = 1.5f;

    public TattooThermalGoggles()
    {
        Id = 8;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Rare;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.AirPieceOpacity += Stat;
        character.CritChancePercent += Mathf.RoundToInt(Stat * _multiplier);
    }

    public override string GetDescription()
    {
        return $"you see air pieces {StatToString("+", "0%")} more distinctively, and increases your critical chance\nby {StatToString(after: "%", statMultiplier: _multiplier)}.";
    }
}
