using UnityEngine;
using System.Collections;

public class TattooBloodEye : Tattoo
{
    public TattooBloodEye()
    {
        Id = 46;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "";
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.HasteCancel = true;
    }

    public override string GetDescription()
    {
        return $"haste attacks don't affect you anymore.";
    }
}