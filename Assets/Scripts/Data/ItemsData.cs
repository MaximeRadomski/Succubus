﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemsData
{
    public static int LegendaryItemAppearancePercent = 5;
    public static int RareItemAppearancePercent = 20;

    public static string[] Items = 
    /* 00 */ { "Holy Water", "Demon Blood", "Grenade", "Voodoo Doll", "Smoke Bomb", "Inner Strength", "Holy Grenade", "Reverse Crucifix", "Wooden Cross", "Forbidden Camembert",
    /* 10 */ "Justice Shovel", "Non-Euclidean Coin", "D6", "Mold Fashioned", "Lucky Ladybug", "Knight Shield", "CRT Monitor", "Creeping Totem", "The Devil", "The World",
    /* 20 */ "The Hierophant", "Eliacube", "Mask of Duality", "Door of Truth", "Sun Juice", "Death Scythe", "Dark Milk", "Atlas Effigy", "Bloodlust Blade", "Moon Gun",
    /* 30 */ "Gender Swap Coffin", "Arcade Button" };

    public static string[] CommonItemsNames =    { Items[00], Items[01], Items[02], Items[03], Items[08], Items[11], Items[12], Items[16], Items[17], Items[21], Items[22], Items[26] };
    public static string[] RareItemsNames =      { Items[04], Items[05], Items[09], Items[10], Items[13], Items[14], Items[15], Items[18], Items[19], Items[20] };
    public static string[] LegendaryItemsNames = { Items[06], Items[07], Items[23], Items[24], Items[25], Items[27], Items[28], Items[29], Items[30] };

    //DEBUG
    public static bool DebugEnabled = Constants.ItemsDebug;
    public static Item DebugItem = GetItemFromName("The Hierophant");

    public static Item GetRandomItem(Rarity maxRarity = Rarity.Legendary)
    {
        var rareBonus = PlayerPrefsHelper.GetBonusRarePercent();
        if (rareBonus > Constants.MaxRarePercent)
            rareBonus = Constants.MaxRarePercent;
        var rarityRare = RareItemAppearancePercent + rareBonus;

        var legendaryBonus = PlayerPrefsHelper.GetBonusLegendaryPercent();
        if (legendaryBonus > Constants.MaxLegendaryPercent)
            legendaryBonus = Constants.MaxLegendaryPercent;
        var rarityLegendary = LegendaryItemAppearancePercent + legendaryBonus;
        

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
