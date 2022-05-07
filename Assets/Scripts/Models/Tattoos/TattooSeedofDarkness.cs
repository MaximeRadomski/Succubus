using UnityEngine;
using System.Collections;

public class TattooSeedofDarkness : Tattoo
{
    private int percentStat = 25;

    public TattooSeedofDarkness()
    {
        Id = 88;
        Name = TattoosData.Tattoos[Id];
        Stat = 4;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.RealmTreeBoost += -(100 / Stat);
        character.DamageFlatBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", " base damage")} but it lowers your skill tree bonuses by {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{percentStat * Level}%{Constants.MaterialEnd}.";
    }
}