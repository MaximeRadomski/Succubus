using UnityEngine;
using System.Collections;

public class TattooDiamond : Tattoo
{
    public TattooDiamond()
    {
        Id = 60;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "any attacks or items making holes";
        Rarity = Rarity.Common;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DiamondBlocks = true;
    }

    public override string GetDescription()
    {
        return $"your blocks now resist {StatToString()} in your playfield.\n(line clearing still works)";
    }
}