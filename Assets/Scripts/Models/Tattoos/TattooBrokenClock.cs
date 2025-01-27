﻿using UnityEngine;
using System.Collections;

public class TattooBrokenClock : Tattoo
{
    public TattooBrokenClock()
    {
        Id = 3;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.LandLordLateAmount += Stat;
    }

    public override string GetDescription()
    {
        return "landlords start to watch over their region " + StatToString("", Stat * Level == 1 ? " step" : " steps") + " later.";
    }
}