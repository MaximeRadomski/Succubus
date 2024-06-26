﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooLastStand : Tattoo
{
    public TattooLastStand()
    {
        Id = 54;
        Name = TattoosData.Tattoos[Id];
        Stat = 6;
        Rarity = Rarity.Legendary;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.LastStandMultiplier += Stat;
    }

    public override string GetDescription()
    {
        return $"if you die, deals {StatToString(after:" times")} your attack as a last stand. if it kills your opponent, you are back in the game. (cannot be used more than once per level)";
    }
}
