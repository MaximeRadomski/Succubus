using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooKinkyBoot : Tattoo
{
    public TattooKinkyBoot()
    {
        Id = 7;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "double jump";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CanDoubleJump = true;
    }

    public override string GetDescription()
    {
        return $"allows you to {StatToString()}!";
    }
}
