using UnityEngine;
using System.Collections;

public class TattooSOPSign : Tattoo
{
    public TattooSOPSign()
    {
        Id = 25;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.MaxDarkAndWasteLines = Stat;
    }

    public override string GetDescription()
    {
        return $"you cannot get more than {StatToString(after: " dark line or waste line")} at a time.";
    }
}