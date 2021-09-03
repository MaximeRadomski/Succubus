using UnityEngine;
using System.Collections;

public class TattooHeavenIcon : Tattoo
{
    public TattooHeavenIcon()
    {
        Id = 29;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "holy";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        if (character.StartingRealm == Realm.None)
            character.StartingRealm = character.Realm;
        character.Realm = Realm.Heaven;
    }

    public override string GetDescription()
    {
        return $"change your nature to a {StatToString()} one.";
    }
}