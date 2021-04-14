using UnityEngine;
using System.Collections;

public class TattooHellSuppo : Tattoo
{
    public TattooHellSuppo()
    {
        Id = 27;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "sinful";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.Realm = Realm.Hell;
    }

    public override string GetDescription()
    {
        return $"Change your nature to a {StatToString()} one.";
    }
}