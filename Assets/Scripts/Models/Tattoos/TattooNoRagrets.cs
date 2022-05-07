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

    protected override void CustomRemove(Character character)
    {
        character.QuadrupleLinesDamageOverride = 0;
    }

    public override string GetDescription()
    {
        return $"your quadruple lines now deal {StatToString("fixed ", " damage")}.";
    }
}