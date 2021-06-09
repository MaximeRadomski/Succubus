using UnityEngine;
using System.Collections;

public class TattooQuadDamage : Tattoo
{
    public TattooQuadDamage()
    {
        Id = 37;
        Name = TattoosData.Tattoos[Id];
        Stat = 4;
        Rarity = Rarity.Common;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.QuadDamage = Stat;
    }

    public override string GetDescription()
    {
        return $"if you face more than {StatToString(after: " enemies")}, you deal {StatToString("x", " more damage")} to the first one.";
    }
}