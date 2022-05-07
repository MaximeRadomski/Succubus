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

    protected override void CustomRemove(Character character)
    {
        character.Realm = character.StartingRealm;
    }

    public override string GetDescription()
    {
        return $"change your nature to a {StatToString()} one.";
    }
}