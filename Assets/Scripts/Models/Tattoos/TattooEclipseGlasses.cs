using UnityEngine;
using System.Collections;

public class TattooEclipseGlasses : Tattoo
{
    public TattooEclipseGlasses()
    {
        Id = 20;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.VisionBlockReducer += Stat;
    }

    public override string GetDescription()
    {
        return $"decreases vision blocks cooldown by {StatToString("", Stat * Level == 1 ? " additional second" : " additional seconds")} each time you lock a piece.";
    }
}