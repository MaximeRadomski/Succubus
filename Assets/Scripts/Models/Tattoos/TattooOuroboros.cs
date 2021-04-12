using UnityEngine;
using System.Collections;

public class TattooOuroboros : Tattoo
{
    public TattooOuroboros()
    {
        Id = 24;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.ItemCooldownReducer += Stat;
    }

    public override string GetDescription()
    {
        return $"opponent attacks reduce your item cooldown by {StatToString("+")}";
    }
}