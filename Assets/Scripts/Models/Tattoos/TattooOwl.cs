using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooOwl : Tattoo
{
    public TattooOwl()
    {
        Id = 58;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.OwlReduceSeconds += Stat;
    }

    public override string GetDescription()
    {
        return $"landing quadruple lines reduces your opponent cooldown progression for {StatToString(after: " seconds")}.";
    }
}
