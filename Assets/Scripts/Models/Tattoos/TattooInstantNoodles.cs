using UnityEngine;
using System.Collections;

public class TattooInstantNoodles : Tattoo
{
    public TattooInstantNoodles()
    {
        Id = 66;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Rare;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.NoodleShield += Stat;
    }

    public override string GetDescription()
    {
        return $"clearing a line by dropping a piece, without any moves or rotations, {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}grants you a shield{Constants.MaterialEnd}.\nup to {StatToString(after: " times")} in a fight.";
    }
}