using UnityEngine;
using System.Collections;

public class TattooWaterEye : Tattoo
{
    public TattooWaterEye()
    {
        Id = 47;
        Name = TattoosData.Tattoos[Id];
        Stat = 20;
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamagePercentBonus += Stat;
        character.HasteForAll = true;
    }

    protected override void CustomRemove(Character character)
    {
        character.HasteForAll = false;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", "%")} damage, but all your opponents now have haste.";
    }
}