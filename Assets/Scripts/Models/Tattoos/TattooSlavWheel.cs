using UnityEngine;
using System.Collections;

public class TattooSlavWheel : Tattoo
{
    public TattooSlavWheel()
    {
        Id = 63;
        Name = TattoosData.Tattoos[Id];
        Stat = 5;
        Rarity = Rarity.Rare;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.SlavWheelDamagePercentBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"each consecutive attack cumulatively deals {StatToString("+", "%")} damage, but each attack has a 1/6 chance to backfire, ending the streak and shrinking your playfield by 1 line.";
    }
}