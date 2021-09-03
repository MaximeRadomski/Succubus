using UnityEngine;
using System.Collections;

public class TattooEarthChild : Tattoo
{
    public TattooEarthChild()
    {
        Id = 28;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "human";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        if (character.StartingRealm == Realm.None)
            character.StartingRealm = character.Realm;
        character.Realm = Realm.Earth;
    }

    public override string GetDescription()
    {
        return $"change your nature to a {StatToString()} one.";
    }
}