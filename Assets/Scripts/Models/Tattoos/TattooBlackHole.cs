using UnityEngine;
using System.Collections;

public class TattooBlackHole : Tattoo
{
    public TattooBlackHole()
    {
        Id = 67;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "all your playfield";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.AllClear = true;

        FillAllSpace();
    }

    protected override void CustomRemove(Character character)
    {
        var tmpStr = PlayerPrefsHelper.GetCurrentTattoosString();
        if (!tmpStr.Contains("BlackHole"))
            character.AllClear = false;
    }

    public override string GetDescription()
    {
        return $"clears {StatToString()} after a fight, but takes all the remaining place on your body.";
    }
}