using UnityEngine;
using System.Collections;

public class TattooAceofClubs : Tattoo
{
    public TattooAceofClubs()
    {
        Id = 83;
        Name = TattoosData.Tattoos[Id];
        Stat = 20;
        Rarity = Rarity.Common;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.TwistBoostedDamage += Stat;
    }

    public override string GetDescription()
    {
        return $"doing a twist buffs you next piece, resulting in {StatToString(after: " damage")} if lines are destroyed with thus piece.";
    }
}