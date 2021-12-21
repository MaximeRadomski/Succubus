using UnityEngine;
using System.Collections;

public class TattooSRS : Tattoo
{
    public TattooSRS()
    {
        Id = 80;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "way more rotations possibilities";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.StupidRotationSystem = true;
    }

    public override string GetDescription()
    {
        return $"stupid rotation system: adds {StatToString()} to your pieces, allowing crazy twists and piece-warping.";
    }
}