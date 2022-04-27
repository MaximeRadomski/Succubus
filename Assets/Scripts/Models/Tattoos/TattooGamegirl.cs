using UnityEngine;
using System.Collections;

public class TattooGamegirl : Tattoo
{
    private float multiplier = 2.5f;

    public TattooGamegirl()
    {
        Id = 93;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamageFlatBonus += Stat;
        character.ChanceOldSchool += Mathf.RoundToInt(Stat * multiplier);
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", " base damage")}, but {StatToString(after: "%", statMultiplier: multiplier)} of your pieces are old school ones.";
    }
}