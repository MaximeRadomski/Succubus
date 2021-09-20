using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooTarget : Tattoo
{
    public TattooTarget()
    {
        Id = 53;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "fills single holes";
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.Target = true;
    }

    public override string GetDescription()
    {
        return $"spawns a target which {StatToString()} in your playfield.";
    }
}
