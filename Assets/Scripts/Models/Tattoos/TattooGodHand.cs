using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooGodHand : Tattoo
{
    public TattooGodHand()
    {
        Id = 51;
        Name = TattoosData.Tattoos[Id];
        Stat = 6;
        Rarity = Rarity.Legendary;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.GodHandCombo += Stat;
    }

    public override string GetDescription()
    {
        return $"deals the equivalent of {StatToString(after:" lines")} damage when you do a x4 combo.";
    }
}
