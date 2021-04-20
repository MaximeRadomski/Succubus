using UnityEngine;
using System.Collections;

public class TattooPermutation : Tattoo
{
    public TattooPermutation()
    {
        Id = 31;
        Name = TattoosData.Tattoos[Id];
        Stat = 10;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DodgeChance += Stat;
    }

    public override string GetDescription()
    {
        return $"gives you {StatToString("+", "%")} chance of dodging attacks.";
    }
}