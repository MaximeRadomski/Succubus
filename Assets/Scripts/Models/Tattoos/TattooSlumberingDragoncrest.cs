using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooSlumberingDragoncrest : Tattoo
{
    public TattooSlumberingDragoncrest()
    {
        Id = 55;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "you start any fight stealthily";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.SlumberingDragoncrest = true;
    }

    public override string GetDescription()
    {
        return $"{StatToString()}, the first opponent won't fight back until you attacked them once.";
    }
}
