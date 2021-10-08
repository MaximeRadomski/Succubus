using UnityEngine;
using System.Collections;

public class TattooBasketballHoop : Tattoo
{
    public TattooBasketballHoop()
    {
        Id = 62;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Rare;
        MaxLevel = 3;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.BasketballHoopTimesBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"spawns a hoop which deals {StatToString(after: " times your damages")} if you destroy a line by hard dropping a piece through it.";
    }
}