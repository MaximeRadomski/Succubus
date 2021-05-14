using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemsData
{
    public static int LegendaryItemAppearancePercent = 5;
    public static int RareItemAppearancePercent = 20;

    public static string[] Items = 
    /* 10 */ { "Holy Water", "Demon Blood", "Grenade", "Voodoo Doll", "Smoke Bomb", "Inner Strength", "Holy Grenade", "Reverse Crucifix", "Wooden Cross", "Forbidden Camembert",
    /* 10 */ "Justice Shovel", "Flipping Coin", "D6", "Mold Fashioned", "Lucky Ladybug", "Knight Shield", "CRT Monitor", "Creeping Totem", "The Devil", "The World",
    /* 20 */ "The Hierophant", "Eliacube", "Mask of Duality", "Door of Truth", "Sun Juice", "Death Scythe", "Dark Milk", "Atlas Effigy" };

    public static string[] CommonItemsNames = { Items[0], Items[1], Items[2], Items[3], Items[8], Items[11], Items[12], Items[16], Items[17], Items[21], Items[22], Items[26] };
    public static string[] RareItemsNames = { Items[4], Items[5], Items[9], Items[10], Items[13], Items[14], Items[15], Items[18], Items[19], Items[20] };
    public static string[] LegendaryItemsNames = { Items[6], Items[7], Items[23], Items[24], Items[25], Items[27] };

    //DEBUG
    public static bool DebugEnabled = true;
    public static Item DebugItem = GetItemFromName("Atlas Effigy");

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
