using UnityEngine;
using System.Collections;

public class TattooFireStone : Tattoo
{
    public TattooFireStone()
    {
        Id = 32;
        Name = TattoosData.Tattoos[Id];
        Stat = 20;
        Rarity = Rarity.Rare;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.FireDamagePercent += Stat;
    }

    public override string GetDescription()
    {
        return $"your triple lines burn your opponent, dealing {StatToString(after:"%")} of your damage, 3 times in 1 second.";
    }
}