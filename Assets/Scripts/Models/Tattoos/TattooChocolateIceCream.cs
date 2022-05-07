using UnityEngine;
using System.Collections;

public class TattooChocolateIceCream : Tattoo
{
    public TattooChocolateIceCream()
    {
        Id = 74;
        Name = TattoosData.Tattoos[Id];
        Stat = 4;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamageFlatBonus += Stat;
        character.QuadrupleLinesDamageOverride = 1;
    }

    protected override void CustomRemove(Character character)
    {
        character.QuadrupleLinesDamageOverride = 0;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", " base damage")}, except for your quadruple lines which now deal {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}1 damage{Constants.MaterialEnd}.";
    }
}