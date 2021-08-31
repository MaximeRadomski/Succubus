using UnityEngine;
using System.Collections;

public class TattooInfiniteStairs : Tattoo
{
    public TattooInfiniteStairs()
    {
        Id = 45;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Legendary;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.EnemyCooldownInfiniteStairMalus += Stat;
    }

    public override string GetDescription()
    {
        return $"your opponent's cooldown increases by {StatToString("+", Stat * Level == 1 ? " second" : " seconds")} on each of their attack.";
    }
}