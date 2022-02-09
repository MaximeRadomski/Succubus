using UnityEngine;
using System.Collections;

public class TattooSeedofLight : Tattoo
{
    public TattooSeedofLight()
    {
        Id = 87;
        Name = TattoosData.Tattoos[Id];
        Stat = 25;
        Rarity = Rarity.Common;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.RealmTreeBoost += Stat;
    }

    public override string GetDescription()
    {
        return $"Boosts your Skill Tree bonuses by {StatToString(after: "%")}.";
    }
}