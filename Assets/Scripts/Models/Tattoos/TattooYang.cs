using UnityEngine;
using System.Collections;

public class TattooYang : Tattoo
{
    public TattooYang()
    {
        Id = 72;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.Yang += Stat;
    }

    public override string GetDescription()
    {
        return $"boosts your attack by {StatToString("+")} while your item's cooldown is over.";
    }
}