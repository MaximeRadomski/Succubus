using UnityEngine;
using System.Collections;

public class TattooWhetstone : Tattoo
{
    public TattooWhetstone()
    {
        Id = 77;
        Name = TattoosData.Tattoos[Id];
        Stat = 25;
        Rarity = Rarity.Common;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CritMultiplier += Stat;
    }

    public override string GetDescription()
    {
        return $"increases your critical damage by {StatToString("+", "%")}.";
    }
}