using UnityEngine;
using System.Collections;

public class TattooIWorship : Tattoo
{
    public TattooIWorship()
    {
        Id = 13;
        Name = TattoosData.Tattoos[Id];
        Stat = 10;
        Rarity = Rarity.Common;
        MaxLevel = 10;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.IWorshipPercent += Stat;
    }

    public override string GetDescription()
    {
        return $"you get {StatToString("", "%")} more I piece.";
    }
}