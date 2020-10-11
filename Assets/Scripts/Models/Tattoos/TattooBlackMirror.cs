using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooBlackMirror : Tattoo
{
    public TattooBlackMirror()
    {
        Id = 6;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "mimic";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CanMimic = true;
    }

    public override string GetDescription()
    {
        return $"going over the limits of the playfield gives your piece the {StatToString()} status!\n{StatToString()} pieces can't rotate but their blocks are individually affected by gravity";
    }
}
