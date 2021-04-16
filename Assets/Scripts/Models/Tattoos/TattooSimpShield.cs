using UnityEngine;
using System.Collections;

public class TattooSimpShield : Tattoo
{
    public TattooSimpShield()
    {
        Id = 30;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.SimpShield += Stat;
    }

    public override string GetDescription()
    {
        return $"gives {StatToString("+")} shield which blocks {StatToString(after: Stat * Level == 1 ? " incoming attack" : " incoming attacks")}.";
    }
}