using UnityEngine;
using System.Collections;

public class TattooKingofClubs : Tattoo
{
    public TattooKingofClubs()
    {
        Id = 96;
        Name = TattoosData.Tattoos[Id];
        Stat = 100;
        Rarity = Rarity.Common;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.TwistDamage += Stat;
    }

    public override string GetDescription()
    {
        return $"each twist now does {StatToString(after: "%")} of your attack.";
    }
}