﻿using UnityEngine;
using System.Collections;

public class TattooHandbrake : Tattoo
{
    public TattooHandbrake()
    {
        Id = 19;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "pauses opponent cooldown";
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.HighPlayPause = true;
    }

    protected override void CustomRemove(Character character)
    {
        character.HighPlayPause = false;
    }

    public override string GetDescription()
    {
        return $"playing high in the playfield, over 17, {StatToString()}.";
    }
}