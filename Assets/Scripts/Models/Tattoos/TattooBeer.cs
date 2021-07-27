using UnityEngine;
using System.Collections;

public class TattooBeer : Tattoo
{
    public TattooBeer()
    {
        Id = 41;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.EnemyMaxCooldownMalus += (float)Stat;
    }

    public override string GetDescription()
    {
        return $"adds {StatToString("", Stat * Level == 1 ? " second" : " seconds")} to opponents cooldowns.";
    }
}