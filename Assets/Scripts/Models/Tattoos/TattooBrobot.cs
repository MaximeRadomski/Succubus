using UnityEngine;
using System.Collections;

public class TattooBrobot : Tattoo
{
    public TattooBrobot()
    {
        Id = 95;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.BrobotStun += Stat;
    }

    public override string GetDescription()
    {
        return $"combos {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}stun{Constants.MaterialEnd} your opponent\nfor {StatToString(after: Stat * Level == 1 ? " second" : " seconds")}.";
    }
}