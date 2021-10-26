using UnityEngine;
using System.Collections;

public class TattooWaterEye : Tattoo
{
    public TattooWaterEye()
    {
        Id = 47;
        Name = TattoosData.Tattoos[Id];
        Stat = 25;
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamagePercentBonus += Stat;
        character.HasteForAll = true;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", "%")} damage, but all your opponents now have haste.";
    }
}