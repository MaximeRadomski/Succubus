using UnityEngine;
using System.Collections;

public class TattooSwordTip : Tattoo
{
    public TattooSwordTip()
    {
        Id = 76;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CumulativeNotCrit += Stat;
    }

    public override string GetDescription()
    {
        return $"for the duration of your current fight, each not critical hit augments your critical hit chances by {StatToString(after: "%")}.";
    }
}