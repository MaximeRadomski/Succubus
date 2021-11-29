using UnityEngine;
using System.Collections;

public class TattooYing : Tattoo
{
    public TattooYing()
    {
        Id = 71;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.Ying += Stat;
    }

    public override string GetDescription()
    {
        return $"boosts your attack by {StatToString("+")} while your special's cooldown is over.";
    }
}