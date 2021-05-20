using UnityEngine;
using System.Collections;

public class TattooDoubleEdged : Tattoo
{
    public TattooDoubleEdged()
    {
        Id = 38;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Rare;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DoubleEdgeGravity += Stat;
        character.DamageFlatBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", " base damages")}, but the gravity also gain {StatToString("+", " levels")}.";
    }
}