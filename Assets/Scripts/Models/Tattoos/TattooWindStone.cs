using UnityEngine;
using System.Collections;

public class TattooWindStone : Tattoo
{
    public TattooWindStone()
    {
        Id = 35;
        Name = TattoosData.Tattoos[Id];
        Stat = 40;
        Rarity = Rarity.Common;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.WaterDamagePercent += Stat;
    }

    public override string GetDescription()
    {
        return $"your triple lines wet your opponent, making him receiving {StatToString("+", "%")} damages for 4 seconds.";
    }
}