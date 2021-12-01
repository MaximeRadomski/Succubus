using UnityEngine;
using System.Collections;

public class TattooSlavWheel : Tattoo
{
    public TattooSlavWheel()
    {
        Id = 63;
        Name = TattoosData.Tattoos[Id];
        Stat = 5;
        Rarity = Rarity.Rare;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.SlavWheelDamagePercentBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"each consecutive attack cumulatively deals {StatToString("+", "%")} damage, but each attack has a {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}1/6 chance{Constants.MaterialEnd} to backfire, ending the streak and adding {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}2 dark rows{Constants.MaterialEnd} to your playfield.";
    }
}