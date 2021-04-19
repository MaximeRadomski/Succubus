﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemsData
{
    public static int LegendaryItemAppearancePercent = 5;
    public static int RareItemAppearancePercent = 25;

    public static string[] Items = { "Holy Water", "Demon Blood", "Grenade", "Voodoo Doll", "Smoke Bomb", "Inner Strength", "Holy Grenade", "Reverse Crucifix", "Wooden Cross", "Forbidden Camembert" };

    public static string[] CommonItemsNames = { Items[0], Items[1], Items[2], Items[3], Items[8] };
    public static string[] RareItemsNames = { Items[4], Items[5], Items[9] };
    public static string[] LegendaryItemsNames = { Items[6], Items[7] };

    //DEBUG
    public static bool DebugEnabled = false;
    public static Item DebugItem = GetItemFromName("Inner Strength");

    public static Item GetRandomItem(Rarity maxRarity = Rarity.Legendary)
    {
        var rarityRare = RareItemAppearancePercent + PlayerPrefsHelper.GetBonusRarePercent();
        if (rarityRare > Constants.MaxRarePercent)
            rarityRare = Constants.MaxRarePercent;
        var rarityLegendary = LegendaryItemAppearancePercent + PlayerPrefsHelper.GetBonusLegendaryPercent();
        if (rarityLegendary > Constants.MaxLegendaryPercent)
            rarityLegendary = Constants.MaxLegendaryPercent;

        int rarityPercent = UnityEngine.Random.Range(0, 100);
        if (rarityPercent < rarityLegendary && maxRarity >= Rarity.Legendary)
            return GetRandomItemFromRarity(Rarity.Legendary);
        else if (rarityPercent < rarityRare && maxRarity >= Rarity.Rare)
            return GetRandomItemFromRarity(Rarity.Rare);
        else
            return GetRandomItemFromRarity(Rarity.Common);
    }

    public static Item GetRandomItemFromRarity(Rarity rarity)
    {
        if (rarity == Rarity.Legendary)
            return GetItemFromNames(LegendaryItemsNames, UnityEngine.Random.Range(0, LegendaryItemsNames.Length));
        else if (rarity == Rarity.Rare)
            return GetItemFromNames(RareItemsNames, UnityEngine.Random.Range(0, RareItemsNames.Length));
        else
            return GetItemFromNames(CommonItemsNames, UnityEngine.Random.Range(0, CommonItemsNames.Length));
    }

    public static Item GetItemFromNames(string[] names, int id, int loopId = -1)
    {
        if (id >= names.Length)
            id = 0;
        if (string.IsNullOrEmpty(names[id]))
            return null;
        var cleanName = names[id].Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        cleanName = cleanName.Replace("-", "");
        var instance = (Item)Activator.CreateInstance(Type.GetType("Item" + cleanName));
        var currentItem = PlayerPrefsHelper.GetCurrentItem();
        if (currentItem != null && currentItem.Id == id && loopId != id)
        {
            if (loopId == -1)
                loopId = id;
            return GetItemFromNames(names, ++id, loopId);
        }
        return instance;
    }

    public static Item GetItemFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        var cleanName = name.Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        cleanName = cleanName.Replace("-", "");
        var instance = Activator.CreateInstance(Type.GetType("Item" + cleanName));
        return (Item)instance;
    }
}
