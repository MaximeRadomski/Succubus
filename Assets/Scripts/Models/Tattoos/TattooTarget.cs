using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooTarget : Tattoo
{
    public TattooTarget()
    {
        Id = 53;
        Name = TattoosData.Tattoos[Id];
        Stat = 4;
        Rarity = Rarity.Rare;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.FillTargetBlocks += Stat;
    }

    public override string GetDescription()
    {
        return $"spawns a target which {StatToString("fills up to ", " single holes")} in your playfield.";
    }
}
