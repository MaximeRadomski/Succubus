using UnityEngine;
using System.Collections;

public class TattooPaperCut : Tattoo
{
    public TattooPaperCut()
    {
        Id = 78;
        Name = TattoosData.Tattoos[Id];
        Stat = 20;
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CritChancePercent += Stat;
        character.DamageFlatBonus -= Stat / 10;
    }

    public override string GetDescription()
    {
        return $"increases your critical chance by {StatToString(after: "%")} but you deal {StatToString("-", " base damage", statMultiplier: 0.1f)}.";
    }
}