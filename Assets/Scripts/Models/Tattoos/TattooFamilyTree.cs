using UnityEngine;
using System.Collections;

public class TattooFamilyTree : Tattoo
{
    public TattooFamilyTree()
    {
        Id = 43;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.RealmPassiveEffect += Stat;
    }

    public override string GetDescription()
    {
        return $"gives your passive effect {StatToString("+", "00%")} efficiency.";
    }
}