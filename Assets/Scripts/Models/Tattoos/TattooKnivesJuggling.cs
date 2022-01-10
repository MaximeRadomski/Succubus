using UnityEngine;
using System.Collections;

public class TattooKnivesJuggling : Tattoo
{
    public TattooKnivesJuggling()
    {
        Id = 81;
        Name = TattoosData.Tattoos[Id];
        Stat = 10;
        Rarity = Rarity.Rare;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.BonusAllowedMovesBeforeLock += Stat;
    }

    public override string GetDescription()
    {
        return $"gives you {StatToString("+")} moves before piece locking.";
    }
}