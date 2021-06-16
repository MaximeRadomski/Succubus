using UnityEngine;
using System.Collections;

public class TattooWaterStone : Tattoo
{
    public TattooWaterStone()
    {
        Id = 34;
        Name = TattoosData.Tattoos[Id];
        Stat = 40;
        Rarity = Rarity.Rare;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.WaterDamagePercent += Stat;
    }

    public override string GetDescription()
    {
        return $"your triple lines wet your opponent, making them receiving {StatToString("+", "%")} damage for 4 seconds.";
    }
}