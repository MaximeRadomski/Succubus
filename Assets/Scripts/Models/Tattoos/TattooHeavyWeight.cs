using UnityEngine;
using System.Collections;
using System;

public class TattooHeavyWeight : Tattoo
{
    public TattooHeavyWeight()
    {
        Id = 23;
        Name = TattoosData.Tattoos[Id];
        Stat = 25;
        Rarity = Rarity.Legendary;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamagePercentBonus += Stat;
        character.PiecesWeight += Stat / Math.Abs(Stat);
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", "%")} damage, but piece locking pounds your playfield!";
    }
}