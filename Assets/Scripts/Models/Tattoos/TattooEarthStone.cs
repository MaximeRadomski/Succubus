using UnityEngine;
using System.Collections;

public class TattooEarthStone : Tattoo
{
    public TattooEarthStone()
    {
        Id = 33;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.EarthStun += Stat;
    }

    public override string GetDescription()
    {
        return $"your triple lines smash your opponent, stunning him for {StatToString()} seconds.";
    }
}