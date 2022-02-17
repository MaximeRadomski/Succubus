using UnityEngine;
using System.Collections;

public class TattooPodium : Tattoo
{
    float _multiCritChance = 2.0f;
    float _multCritDamage = 3.0f;

    public TattooPodium()
    {
        Id = 90;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamageFlatBonus += Stat;
        character.CritChancePercent += Mathf.RoundToInt(Stat * _multiCritChance);
        character.CritMultiplier += Mathf.RoundToInt(Stat * _multCritDamage);
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", " base damage")},\n" +
            $"also increases your critical chance by {StatToString(after: "%", statMultiplier: _multiCritChance)},\n" +
            $"and your critical damage by {StatToString("+", "%", statMultiplier: _multCritDamage)}.";
    }
}