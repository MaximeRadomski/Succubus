using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemsData
{
    public static int LegendaryItemAppearancePercent = 10;
    public static int RareItemAppearancePercent = 30;

    public static string[] CommonItemsNames = { "Holy Water", "Demon Blood", "Grenade" };
    public static string[] RareItemsNames = { "Holy Water" };
    public static string[] LegendaryItemsNames = { "Holy Water" };

    public static Item GetRandomItem()
    {
        int rarityPercent = UnityEngine.Random.Range(0, 100);
        if (rarityPercent < LegendaryItemAppearancePercent)
            return GetRandomItemFromRarity(Rarity.Legendary);
        else if (rarityPercent < RareItemAppearancePercent)
            return GetRandomItemFromRarity(Rarity.Rare);
        return GetRandomItemFromRarity(Rarity.Common);
    }

    public static Item GetRandomItemFromRarity(Rarity rarity)
    {
        if (rarity == Rarity.Legendary)
            return GetItemFromName(LegendaryItemsNames[UnityEngine.Random.Range(0, LegendaryItemsNames.Length)]);
        else if (rarity == Rarity.Rare)
            return GetItemFromName(RareItemsNames[UnityEngine.Random.Range(0, RareItemsNames.Length)]);
        return GetItemFromName(CommonItemsNames[UnityEngine.Random.Range(0, CommonItemsNames.Length)]);
    }

    public static Item GetItemFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        var cleanName = name.Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        var instance = Activator.CreateInstance(Type.GetType("Item" + cleanName));
        return (Item)instance;
    }
}
