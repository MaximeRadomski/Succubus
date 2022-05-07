using UnityEngine;
using System.Collections;

public class TattooStar : Tattoo
{
    public TattooStar()
    {
        Id = 44;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "opponent attacks won't trigger";
        Rarity = Rarity.Common;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.LineDestroyInvulnerability = true;
    }

    protected override void CustomRemove(Character character)
    {
        character.LineDestroyInvulnerability = false;
    }

    public override string GetDescription()
    {
        return $"{StatToString()} anymore if your last piece dropped cleared a line.\n(doesn't cancel the attack, just delays it)";
    }
}