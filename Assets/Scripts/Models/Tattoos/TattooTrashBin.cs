using UnityEngine;
using System.Collections;

public class TattooTrashBin : Tattoo
{
    public TattooTrashBin()
    {
        Id = 17;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DeleteAfterKill += Stat;
    }

    public override string GetDescription()
    {
        return $"clears {StatToString("", Stat == 1 ? " line" : " lines")} from the bottom after each opponent death.";
    }
}