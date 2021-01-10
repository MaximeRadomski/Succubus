using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemsData
{
    public static int LegendaryItemAppearancePercent = 10;
    public static int RareItemAppearancePercent = 30;

    public static string[] Items = { "Holy Water", "Demon Blood", "Grenade", "Voodoo Doll", "Smoke Bomb", "Inner Strength", "Holy Grenade", "Reverse Crucifix", "Wooden Cross", "Forbidden Camembert" };

    public static string[] CommonItemsNames = { Items[0], Items[1], Items[2], Items[3] };
    public static string[] RareItemsNames = { Items[4], Items[8] };
    public static string[] LegendaryItemsNames = { Items[5], Items[6], Items[7] };

    //DEBUG
    public static bool DebugEnabled = false;
    public static Item DebugItem = GetItemFromName("Forbidden Camembert");

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
