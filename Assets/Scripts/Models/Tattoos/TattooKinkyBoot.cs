using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooKinkyBoot : Tattoo
{
    private int thornMultiplier = 50;

    public TattooKinkyBoot()
    {
        Id = 7;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.GateWidener += Stat;
        character.ThornsPercent += thornMultiplier;
    }

    public override string GetDescription()
    {
        return $"increases gate attacks wideness by {StatToString(after: Stat * Level == 1 ? " block" : " blocks")},\nand hurts your opponent everytime it attacks by {StatToString(after: "%", statMultiplier: thornMultiplier)} of your attack.";
    }
}
