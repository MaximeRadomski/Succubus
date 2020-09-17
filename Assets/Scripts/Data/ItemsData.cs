using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemsData
{
    public static int RareItemAppearancePercent = 10;
    public static int MagicalItemAppearancePercent = 30;

    public static string[] NormalItemsNames = { "Holy Water", "Demon Blood", "Grenade" };
    public static string[] MagicalItemsNames = { "" };
    public static string[] RareItemsNames = { "" };

    public static Item GetRandomItem()
    {
        int rarityPercent = UnityEngine.Random.Range(0, 100);
        if (rarityPercent < RareItemAppearancePercent)
            return GetRandomItemFromRarity(OpponentType.Champion);
        else if (rarityPercent < MagicalItemAppearancePercent)
            return GetRandomItemFromRarity(OpponentType.Elite);
        return GetRandomItemFromRarity(OpponentType.Common);
    }

    public static Item GetRandomItemFromRarity(OpponentType rarity)
    {
        if (rarity == OpponentType.Champion)
            return GetItemFromName(RareItemsNames[UnityEngine.Random.Range(0, RareItemsNames.Length)]);
        else if (rarity == OpponentType.Elite)
            return GetItemFromName(MagicalItemsNames[UnityEngine.Random.Range(0, MagicalItemsNames.Length)]);
        return GetItemFromName(NormalItemsNames[UnityEngine.Random.Range(0, NormalItemsNames.Length)]);
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
