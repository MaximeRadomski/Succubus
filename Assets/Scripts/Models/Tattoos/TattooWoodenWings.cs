using UnityEngine;
using System.Collections;

public class TattooWoodenWings : Tattoo
{
    public TattooWoodenWings()
    {
        Id = 11;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 10;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.LoweredGravity += Stat;
    }

    public override string GetDescription()
    {
        return $"lowers gravity strength by {StatToString(after:" levels")}.";
    }
}