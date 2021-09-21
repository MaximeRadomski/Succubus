using UnityEngine;
using System.Collections;

public class TattooDamocles : Tattoo
{
    public TattooDamocles()
    {
        Id = 39;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 10;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamoclesDamage += Stat;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+")} damage each time you land quadruple lines for the duration of the fight.";
    }
}