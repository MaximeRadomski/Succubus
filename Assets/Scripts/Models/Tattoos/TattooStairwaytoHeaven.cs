using UnityEngine;
using System.Collections;

public class TattooStairwaytoHeaven : Tattoo
{
    public TattooStairwaytoHeaven()
    {
        Id = 94;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.BonusLife += Stat;
        character.StairwayToHeaven = true;
    }

    public override string GetDescription()
    {
        return $"gives you {StatToString("+", " lives")}, but you don't get to refuse loot or choose which step to go to anymore.";
    }
}