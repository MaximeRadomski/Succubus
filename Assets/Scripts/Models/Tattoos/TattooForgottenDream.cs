using UnityEngine;
using System.Collections;

public class TattooForgottenDream : Tattoo
{
    public TattooForgottenDream()
    {
        Id = 36;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DamageFlatBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"you deal {StatToString("+", " base damages")}.\nYup... That's it...";
    }
}