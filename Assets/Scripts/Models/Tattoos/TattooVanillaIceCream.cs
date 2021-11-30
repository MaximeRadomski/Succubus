using UnityEngine;
using System.Collections;

public class TattooVanillaIceCream : Tattoo
{
    public TattooVanillaIceCream()
    {
        Id = 73;
        Name = TattoosData.Tattoos[Id];
        Stat = 50;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamagePercentBonus += Stat;
        character.SingleLinesDamageOverride = 1;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", "%")} damage, except for your single lines which now deal {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}1 damage{Constants.MaterialEnd}.";
    }
}