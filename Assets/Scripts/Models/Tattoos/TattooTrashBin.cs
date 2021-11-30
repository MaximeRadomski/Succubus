using UnityEngine;
using System.Collections;

public class TattooTrashBin : Tattoo
{
    public TattooTrashBin()
    {
        Id = 17;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 10;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.TrashAfterKill += Stat;
    }

    public override string GetDescription()
    {
        return $"clears {StatToString("", Stat * Level == 1 ? " line" : " lines")} from the bottom after each opponent death.";
    }
}