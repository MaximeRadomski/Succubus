﻿using UnityEngine;
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
        character.Realm = Realm.Earth;
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