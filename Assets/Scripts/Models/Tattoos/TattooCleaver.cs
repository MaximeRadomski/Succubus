using UnityEngine;
using System.Collections;

public class TattooCleaver : Tattoo
{
    public TattooCleaver()
    {
        Id = 2;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Rare;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.ItemCooldownReducerOnKill += Stat;
    }

    public override string GetDescription()
    {
        return "killing an opponent reduces your item cooldown by " + StatToString();
    }
}