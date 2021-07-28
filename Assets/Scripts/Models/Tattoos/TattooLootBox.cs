using UnityEngine;
using System.Collections;

public class TattooLootBox : Tattoo
{
    public TattooLootBox()
    {
        Id = 42;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "from the beginning";
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.InstantSpecial = true;
    }

    public override string GetDescription()
    {
        return $"allows you to use your special {StatToString()} of any fight.";
    }
}