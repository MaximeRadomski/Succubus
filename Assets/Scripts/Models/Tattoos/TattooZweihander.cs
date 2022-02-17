using UnityEngine;
using System.Collections;

public class TattooZweihander : Tattoo
{
    public TattooZweihander()
    {
        Id = 92;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamageFlatBonus += Stat;
        character.LoweredGravity += Stat;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", " base damage")}, and lower gravity strength by {StatToString(after: Stat * Level == 1 ? " level":  " levels")}.";
    }
}