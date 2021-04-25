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
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamoclesDamages += Stat;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+")} damage each time you land a 4 lines for the duration of the fight.";
    }
}