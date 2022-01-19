using UnityEngine;
using System.Collections;

public class TattooScissorJack : Tattoo
{
    public TattooScissorJack()
    {
        Id = 61;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Common;
        MaxLevel = 10;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CancelableShrinkingLines += Stat;
    }

    public override string GetDescription()
    {
        return $"cancels the first {StatToString(after: " shrinking lines")} in a fight.";
    }
}