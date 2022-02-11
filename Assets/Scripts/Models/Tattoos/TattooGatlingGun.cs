using UnityEngine;
using System.Collections;

public class TattooGatlingGun : Tattoo
{
    public TattooGatlingGun()
    {
        Id = 89;
        Name = TattoosData.Tattoos[Id];
        Stat = 20;
        Rarity = Rarity.Common;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.GatlingPercentDamage += Stat;
    }

    public override string GetDescription()
    {
        return $"deals {StatToString(after: "%")} of your attack every {Constants.CookiePiecesMax} pieces locked.";
    }
}