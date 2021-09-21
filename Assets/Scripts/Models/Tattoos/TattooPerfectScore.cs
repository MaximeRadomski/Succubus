using UnityEngine;
using System.Collections;

public class TattooPerfectScore : Tattoo
{
    public TattooPerfectScore()
    {
        Id = 22;
        Name = TattoosData.Tattoos[Id];
        Stat = 100;
        Rarity = Rarity.Legendary;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.PerfectKills += Stat;
    }

    public override string GetDescription()
    {
        return $"your perfect-clears deal {StatToString(after:" damage")} to the opponent.";
    }
}