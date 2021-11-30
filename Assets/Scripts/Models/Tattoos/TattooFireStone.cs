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
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.FireDamagePercent += Stat;
    }

    public override string GetDescription()
    {
        return $"your triple lines burn your opponent, dealing {StatToString(after:"%")} of your attack, 3 times in 1 second.";
    }
}