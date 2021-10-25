using UnityEngine;
using System.Collections;

public class TattooBassGuitar : Tattoo
{
    public TattooBassGuitar()
    {
        Id = 65;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.BassGuitarBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"allows you to play only good notes and in perfect rhythm for the {StatToString("next ", " music based attacks")}.";
    }
}