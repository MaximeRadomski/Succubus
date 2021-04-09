using UnityEngine;
using System.Collections;

public class TattooBackspaceKey : Tattoo
{
    public TattooBackspaceKey()
    {
        Id = 14;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.EnemyCooldownProgressionReducer += Stat;
    }

    public override string GetDescription()
    {
        return $"your attacks reduce your opponent cooldown progression by {StatToString("", Stat == 1 ? " second" : " seconds")}.";
    }
}