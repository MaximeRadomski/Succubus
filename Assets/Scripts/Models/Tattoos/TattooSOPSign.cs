using UnityEngine;
using System.Collections;

public class TattooSOPSign : Tattoo
{
    public TattooSOPSign()
    {
        Id = 25;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.MaxDarkAndWasteLines = Stat;
    }

    public override string GetDescription()
    {
        return $"the amount of dark lines or waste lines you take is {StatToString("divided by ")}.";
    }
}