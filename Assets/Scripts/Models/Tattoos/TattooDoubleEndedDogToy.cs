using UnityEngine;
using System.Collections;

public class TattooDoubleEndedDogToy : Tattoo
{
    public TattooDoubleEndedDogToy()
    {
        Id = 79;
        Name = TattoosData.Tattoos[Id];
        Stat = 5;
        Rarity = Rarity.Rare;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CritChancePercent += Stat;
        character.CritMultiplier += (Stat * 3);
    }

    public override string GetDescription()
    {
        return $"increases your critical chance by {StatToString(after: "%")} and your critical damage by {StatToString("+", "%", statMultiplier: 3.0f)}.";
    }
}