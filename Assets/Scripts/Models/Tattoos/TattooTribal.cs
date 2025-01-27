﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooTribal : Tattoo
{
    public TattooTribal()
    {
        Id = 0;
        Name = TattoosData.Tattoos[Id];
        Stat = 15;
        Rarity = Rarity.Common;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamagePercentBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", "%")} damage.";
    }
}
