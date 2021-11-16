using UnityEngine;
using System.Collections;

public class TattooLogicGate : Tattoo
{
    public TattooLogicGate()
    {
        Id = 70;
        Name = TattoosData.Tattoos[Id];
        Stat = 4;
        Rarity = Rarity.Common;
        MaxLevel = 10;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.HeldBoosted += Stat;
    }

    public override string GetDescription()
    {
        return $"boosts the next {StatToString(after: " held pieces")}, giving them gravity blocks.";
    }
}