﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDoorofTruth : Item
{
    public ItemDoorofTruth()
    {
        Id = 23;
        Name = ItemsData.Items[Id];
        Description = $"breaks on use, but {Highlight("resurrects you")} if you die during the fight in which you used it.";
        Rarity = Rarity.Legendary;
        Cooldown = -1;
        IsUsesBased = true;
        Uses = 1;
    }

    protected override object Effect()
    {
        Cache.TruthResurrection = true;
        PlayerPrefsHelper.ResetCurrentItem();
        _gameplayControler.CharacterItem = PlayerPrefsHelper.GetCurrentItem();
        _gameplayControler.UpdateItemAndSpecialVisuals();
        return base.Effect();
    }
}
