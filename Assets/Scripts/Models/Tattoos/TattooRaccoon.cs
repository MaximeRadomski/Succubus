using UnityEngine;
using System.Collections;

public class TattooRaccoon : Tattoo
{
    public TattooRaccoon()
    {
        Id = 97;
        Name = TattoosData.Tattoos[Id];
        Stat = 5;
        Rarity = Rarity.Rare;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamageFlatBonus += Stat;
        character.RaccoonWaste = true;
    }

    protected override void CustomRemove(Character character)
    {
        character.RaccoonWaste = false;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", " base damage")} but your playfield isn't cleared by beating a boss or by the lurker anymore.";
    }
}