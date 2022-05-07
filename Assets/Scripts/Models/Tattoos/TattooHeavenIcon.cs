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
        character.Realm = Realm.Heaven;
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