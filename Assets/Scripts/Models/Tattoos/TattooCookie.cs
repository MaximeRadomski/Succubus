﻿using UnityEngine;
using System.Collections;

public class TattooCookie : Tattoo
{
    public TattooCookie()
    {
        Id = 84;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CookieSpecialBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"your special loses {StatToString(after: Stat * Level == 1 ? " cooldown point": " cooldown points")} every {Constants.CookiePiecesMax} pieces locked.";
    }
}