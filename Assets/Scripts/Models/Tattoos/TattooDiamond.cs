using UnityEngine;
using System.Collections;

public class TattooDiamond : Tattoo
{
    public TattooDiamond()
    {
        Id = 60;
        Name = TattoosData.Tattoos[Id];
        Stat = 4;
        Rarity = Rarity.Common;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DiamondBlocks += Stat;
    }

    public override string GetDescription()
    {
        return $"cancels the first {StatToString(after: " attacks or items")} making holes in your playfield for a fight.";
    }
}