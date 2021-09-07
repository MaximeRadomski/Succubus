using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooHammer : Tattoo
{
    public TattooHammer()
    {
        Id = 50;
        Name = TattoosData.Tattoos[Id];
        Stat = 30;
        Rarity = Rarity.Common;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamageBigLinesBonus += Stat;
        character.DamageSmallLinesMalus += Stat / 2;
    }

    public override string GetDescription()
    {
        return $"your triple and quadruple lines make {StatToString("+", "%")} damage.\nyour single and double lines make {StatToString("-", "%", 0.5f)} damage.";
    }
}
