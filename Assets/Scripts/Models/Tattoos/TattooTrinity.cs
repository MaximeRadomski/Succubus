﻿using UnityEngine;
using System.Collections;

public class TattooTrinity : Tattoo
{
    public TattooTrinity()
    {
        Id = 1;
        Name = TattoosData.Tattoos[Id];
        Stat = 10;
        Rarity = Rarity.Common;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamagePercentToInferiorRealm += Stat;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", "%")} damage to your inferior nature.";
    }
}