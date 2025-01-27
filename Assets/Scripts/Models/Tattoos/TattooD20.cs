﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class TattooD20 : Tattoo
{
    public TattooD20()
    {
        Id = 9;
        Name = TattoosData.Tattoos[Id];
        Stat = 4;
        Rarity = Rarity.Common;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CritChancePercent += Stat;
    }

    public override string GetDescription()
    {
        return $"increases your critical chance\nby {StatToString(after: "%")}.";
    }
}
