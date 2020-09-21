using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooTribal : Tattoo
{
    public TattooTribal()
    {
        Id = 0;
        Name = "Tribal";
        Stat = 10;
        Description = "you deal " + StatToString("+", "%") + " damages.";
        Rarity = Rarity.Common;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        base.ApplyToCharacter(character);
        character.DamagePercentBonus += Stat;
    }
}
