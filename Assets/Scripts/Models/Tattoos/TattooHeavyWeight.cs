using UnityEngine;
using System.Collections;

public class TattooHeavyWeight : Tattoo
{
    public TattooHeavyWeight()
    {
        Id = 23;
        Name = TattoosData.Tattoos[Id];
        Stat = 50;
        Rarity = Rarity.Legendary;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamagePercentBonus += Stat;
        character.PiecesWeight += 1;

    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", "%")} damages, but each time you hard drop a piece from more than 2 lines, it blocks your gameplay!";
    }
}