using UnityEngine;
using System.Collections;

public class TattooNoRagrets : Tattoo
{
    public TattooNoRagrets()
    {
        Id = 75;
        Name = TattoosData.Tattoos[Id];
        Stat = 69;
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.QuadrupleLinesDamageOverride = Stat;
    }

    public override string GetDescription()
    {
        return $"your quadruple lines now deal {StatToString("fixed ", " damage")}.";
    }
}