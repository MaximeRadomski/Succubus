using UnityEngine;
using System.Collections;

public class TattooDevilsContract : Tattoo
{
    public TattooDevilsContract()
    {
        Id = 64;
        Name = TattoosData.Tattoos[Id];
        Stat = 2;
        Rarity = Rarity.Rare;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DevilsContractMalus += Stat;
    }

    public override string GetDescription()
    {
        return $"your opponent get {StatToString("+", " seconds")} of cooldown, but you get {StatToString("-", " pieces")} in your piece preview.";
    }
}