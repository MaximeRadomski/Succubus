using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooSickle : Tattoo
{
    public TattooSickle()
    {
        Id = 49;
        Name = TattoosData.Tattoos[Id];
        Stat = 30;
        Rarity = Rarity.Common;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamageSmallLinesBonus += Stat;
        character.DamageBigLinesMalus += Stat / 2;
    }

    public override string GetDescription()
    {
        return $"your single and double lines deal {StatToString("+", "%")} damage.\nyour triple and quadruple lines deal {StatToString("-", "%", 0.5f)} damage.";
    }
}
