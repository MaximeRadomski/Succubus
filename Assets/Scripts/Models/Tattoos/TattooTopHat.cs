using UnityEngine;
using System.Collections;

public class TattooTopHat : Tattoo
{
    public TattooTopHat()
    {
        Id = 82;
        Name = TattoosData.Tattoos[Id];
        Stat = 5;
        Rarity = Rarity.Common;
        MaxLevel = 10;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.NegateAttackBoost += Stat;
    }

    public override string GetDescription()
    {
        return $"negates the first {StatToString(after: " buffed attacks")} from opponents of your dominant realm in a fight.";
    }
}