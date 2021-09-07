using UnityEngine;
using System.Collections;

public class TattooInsulatingFoam : Tattoo
{
    public TattooInsulatingFoam()
    {
        Id = 48;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 8;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.WasteHoleFiller += Stat;
    }

    public override string GetDescription()
    {
        return $"fills you waste rows holes by {StatToString("+", Stat * Level == 1 ? " column" : " columns")}.";
    }
}