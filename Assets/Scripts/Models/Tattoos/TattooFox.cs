using UnityEngine;
using System.Collections;

public class TattooFox : Tattoo
{
    public TattooFox()
    {
        Id = 5;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.ItemMaxCooldownReducer += Stat;
    }

    public override string GetDescription()
    {
        return $"your items total cooldown are reduced by {StatToString()}.";
    }
}