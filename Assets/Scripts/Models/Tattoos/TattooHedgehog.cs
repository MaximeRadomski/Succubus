using UnityEngine;
using System.Collections;

public class TattooHedgehog : Tattoo
{
    public TattooHedgehog()
    {
        Id = 18;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 10;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.SpecialTotalCooldownReducer += Stat;
    }

    public override string GetDescription()
    {
        return $"reduces your special's total cooldown by {StatToString()}.";
    }
}