﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TattooBrokenSword : Tattoo
{
    public TattooBrokenSword()
    {
        Id = 10;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CumulativeCrit += Stat;
    }

    public override string GetDescription()
    {
        return $"for the duration of your current fight, each critical hit augments your critical hit chances by {StatToString(after:"%")}.";
    }
}
