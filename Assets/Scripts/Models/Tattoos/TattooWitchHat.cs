using UnityEngine;
using System.Collections;

public class TattooWitchHat : Tattoo
{
    private float multiplier = 5.0f;

    public TattooWitchHat()
    {
        Id = 85;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamageFlatBonus += Stat;
        character.ChanceLesserBlock += Mathf.RoundToInt(Stat * multiplier);
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", " base damage")}, but {StatToString(after: "%", statMultiplier: multiplier)} of your pieces get one lesser block.";
    }
}