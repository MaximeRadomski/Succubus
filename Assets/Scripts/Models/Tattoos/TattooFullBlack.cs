using UnityEngine;
using System.Collections;

public class TattooFullBlack : Tattoo
{
    public TattooFullBlack()
    {
        Id = 26;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.BonusLife += Stat;

        FillAllSpace();
    }

    public override string GetDescription()
    {
        return $"gives you {StatToString("+", " life")}, but takes all the remaining place on your body.";
    }
}