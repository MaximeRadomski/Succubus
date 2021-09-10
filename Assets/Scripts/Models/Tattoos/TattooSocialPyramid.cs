using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooSocialPyramid : Tattoo
{
    public TattooSocialPyramid()
    {
        Id = 56;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "common and elite";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.SocialPyramid = true;
    }

    public override string GetDescription()
    {
        return $"reduces the opponents you encounter to only {StatToString()} ones.";
    }
}
