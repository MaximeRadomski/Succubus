﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDoorofTruth : Item
{
    public ItemDoorofTruth()
    {
        Id = 23;
        Name = ItemsData.Items[Id];
        Description = $"breaks on use, but {Highlight("resurrects you")} up to 3 times if you die during the fight in which you used it.";
        Rarity = Rarity.Legendary;
        Cooldown = -1;
        Type = ItemType.UsesBased;
        Uses = 1;
    }

    protected override void Effect()
    {
        Cache.TruthResurrection = 3;
        PlayerPrefsHelper.ResetCurrentItem();
        _gameplayControler.CharacterItem = PlayerPrefsHelper.GetCurrentItem();
        _gameplayControler.UpdateItemAndSpecialVisuals();
        base.Effect();
    }
}
