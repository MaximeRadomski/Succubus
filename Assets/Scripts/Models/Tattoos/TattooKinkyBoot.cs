using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooKinkyBoot : Tattoo
{
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
        character.CanDoubleJump = true;
    }

    public override string GetDescription()
    {
        return $"increases gate attacks wideness by {StatToString(after: Stat * Level == 1 ? " block" : " blocks")}";
    }
}
