using UnityEngine;
using System.Collections;

public class TattooButterflyKnife : Tattoo
{
    public TattooButterflyKnife()
    {
        Id = 68;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Rare;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.InstantLineClear += Stat;
    }

    public override string GetDescription()
    {
        return $"doing a twist, without any line clearing, clears up to {StatToString("", Stat * Level == 1 ? " dark or waste row" : " dark or waste rows")}.";
    }
}