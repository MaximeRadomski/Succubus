using UnityEngine;
using System.Collections;

public class TattooCrownOfThorns : Tattoo
{
    public TattooCrownOfThorns()
    {
        Id = 21;
        Name = TattoosData.Tattoos[Id];
        Stat = 20;
        Rarity = Rarity.Common;
        MaxLevel = 5;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.ThornsPercent += Stat;
    }

    public override string GetDescription()
    {
        return $"hurts your opponent everytime it attacks by {StatToString(after:"%")} of your attack.";
    }
}