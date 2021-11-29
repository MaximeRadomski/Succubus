using UnityEngine;
using System.Collections;

public class TattooHolyMantle : Tattoo
{
    public TattooHolyMantle()
    {
        Id = 69;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.HolyMantle += Stat;
    }

    public override string GetDescription()
    {
        return $"nullify the first {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{(Stat * Level == 1 ? "attack" : $"{Stat * Level} attacks")}{Constants.MaterialEnd} of each opponent.";
    }
}