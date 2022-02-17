using UnityEngine;
using System.Collections;

public class TattooWhacAMole : Tattoo
{
    public TattooWhacAMole()
    {
        Id = 91;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.WhacAMoleStrength += Stat;
    }

    public override string GetDescription()
    {
        return $"lowers gravity by {StatToString(after: Stat * Level == 1 ? " level" : " levels")} every {Constants.WhacAMoleMax} attacks.";
    }
}