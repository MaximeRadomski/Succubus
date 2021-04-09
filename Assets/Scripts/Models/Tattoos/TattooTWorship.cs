using UnityEngine;
using System.Collections;

public class TattooTWorship : Tattoo
{
    public TattooTWorship()
    {
        Id = 12;
        Name = TattoosData.Tattoos[Id];
        Stat = 10;
        Rarity = Rarity.Common;
        MaxLevel = 10;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.TWorshipPercent += Stat;
    }

    public override string GetDescription()
    {
        return $"you get {StatToString("", "%")} more T piece.";
    }
}