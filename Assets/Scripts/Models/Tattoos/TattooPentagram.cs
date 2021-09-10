using UnityEngine;
using System.Collections;

public class TattooPentagram : Tattoo
{
    private float multiplier = 0.50f;

    public TattooPentagram()
    {
        Id = 59;
        Name = TattoosData.Tattoos[Id];
        Stat = 20;
        Rarity = Rarity.Rare;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamagePercentBonus += Stat;
        character.ChanceAdditionalBlock += Mathf.RoundToInt(Stat * multiplier);
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", "%")} damage, but {StatToString(after: "%", statMultiplier: multiplier)} of your pieces get an additional block.";
    }
}