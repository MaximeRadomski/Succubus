using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooBrandofSacrifice : Tattoo
{
    public TattooBrandofSacrifice()
    {
        Id = 40;
        Name = TattoosData.Tattoos[Id];
        Stat = 50;
        Rarity = Rarity.Rare;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamagePercentBonus += Stat;
        character.StepsWeightMalus += Stat / 2;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", "%")} damage, but you also face more challenge on each step.\n(more opponents or more powerful ones)";
    }
}
