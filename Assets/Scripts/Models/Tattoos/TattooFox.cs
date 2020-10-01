using UnityEngine;
using System.Collections;

public class TattooFox : Tattoo
{
    public TattooFox()
    {
        Id = 5;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Rare;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.ItemMaxCooldownReducer += Stat;
    }

    public override string GetDescription()
    {
        return "your item Max cooldown is reduced by " + StatToString();
    }
}