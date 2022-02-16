using UnityEngine;
using System.Collections;

public class TattooInsulatingFoam : Tattoo
{
    public TattooInsulatingFoam()
    {
        Id = 48;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.WasteHoleFiller += Stat;
    }

    public override string GetDescription()
    {
        return $"fills your waste rows holes by {StatToString("+", " columns")}.";
    }
}