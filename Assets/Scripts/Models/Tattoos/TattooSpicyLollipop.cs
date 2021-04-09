using UnityEngine;
using System.Collections;

public class TattooSpicyLollipop : Tattoo
{
    public TattooSpicyLollipop()
    {
        Id = 16;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.ComboDarkRow += Stat;
    }

    public override string GetDescription()
    {
        return $"each combo destroys {StatToString("", Stat == 1 ? " dark row" : " dark rows")}.";
    }
}