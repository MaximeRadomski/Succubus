using UnityEngine;
using System.Collections;

public class TattooWindStone : Tattoo
{
    public TattooWindStone()
    {
        Id = 35;
        Name = TattoosData.Tattoos[Id];
        Stat = 3;
        Rarity = Rarity.Rare;
        MaxLevel = 99;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.WindTripleBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"your triple lines boost your triple lines by {StatToString("+", " damage")} for the rest of the fight.";
    }
}