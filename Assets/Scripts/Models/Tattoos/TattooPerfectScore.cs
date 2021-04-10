using UnityEngine;
using System.Collections;

public class TattooPerfectScore : Tattoo
{
    public TattooPerfectScore()
    {
        Id = 22;
        Name = TattoosData.Tattoos[Id];
        Stat = 9999;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.PerfectKills = true;
    }

    public override string GetDescription()
    {
        return $"your perfect-clears deal {StatToString(after:" damages")} to the opponent.";
    }
}