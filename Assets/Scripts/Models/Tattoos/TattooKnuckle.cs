using UnityEngine;
using System.Collections;

public class TattooKnuckle : Tattoo
{
    public TattooKnuckle()
    {
        Id = 4;
        Name = TattoosData.Tattoos[Id];
        Stat = 4;
        Rarity = Rarity.Common;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.SingleLineDamageBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"your single lines deal {StatToString("+")} damages.";
    }
}