using UnityEngine;
using System.Collections;

public class TattooXRayGlasses : Tattoo
{
    public TattooXRayGlasses()
    {
        Id = 15;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "through";
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.XRay = true;
    }

    public override string GetDescription()
    {
        return $"allows you to see your pieces' shadows {StatToString()} vision blocks.";
    }
}