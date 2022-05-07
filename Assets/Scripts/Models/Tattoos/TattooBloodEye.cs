using UnityEngine;
using System.Collections;

public class TattooBloodEye : Tattoo
{
    public TattooBloodEye()
    {
        Id = 46;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "don't affect you";
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.HasteCancel = true;
    }

    protected override void CustomRemove(Character character)
    {
        character.HasteCancel = false;
    }

    public override string GetDescription()
    {
        return $"haste attacks {StatToString()} anymore.";
    }
}