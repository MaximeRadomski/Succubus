using UnityEngine;
using System.Collections;

public class TattooPenroseTriangle : Tattoo
{
    public TattooPenroseTriangle()
    {
        Id = 98;
        Name = TattoosData.Tattoos[Id];
        Stat = 100;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.PenroseTriangle = true;
    }

    protected override void CustomRemove(Character character)
    {
        character.PenroseTriangle = false;
    }

    public override string GetDescription()
    {
        return $"your damage fluctuates between {StatToString("-", "%")} and {StatToString("+", "%", 3)} after each attack.";
    }
}