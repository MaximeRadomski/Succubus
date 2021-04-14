using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TattoosData
{
    public static int LegendaryTattooAppearancePercent = 1;
    public static int RareTattooAppearancePercent = 15;

    public static string[] Tattoos =
        { "Tribal", "Trinity", "Cleaver", "Broken Clock", "Knuckle", "Fox", "Black Mirror", "Kinky Boot", "Thermal Goggles", "D20",
        "Broken Sword", "Wooden Wings", "T Worship", "I Worship", "Backspace Key", "X-Ray Glasses", "Spicy Lollipop", "Trash Bin", "Hedgehog", "Handbrake",
        "Eclipse Glasses", "Crown Of Thorns", "Perfect Score", "Heavy Weight", "Ouroboros", "SOP Sign", "Full Black" };

    public static string[] CommonTattoosNames = { Tattoos[0], Tattoos[1], Tattoos[4], Tattoos[9], Tattoos[10], Tattoos[11], Tattoos[12], Tattoos[13], Tattoos[17], Tattoos[20], Tattoos[21], Tattoos[24] };
    public static string[] RareTattoosNames = { Tattoos[2], Tattoos[3], Tattoos[5], Tattoos[8], Tattoos[14], Tattoos[15], Tattoos[16], Tattoos[18], Tattoos[19] };
    public static string[] LegendaryTattoosNames = { Tattoos[6], Tattoos[7], Tattoos[22], Tattoos[23], Tattoos[25], Tattoos[26] };

    //DEBUG
    public static bool DebugEnabled = false;
    public static Tattoo DebugTattoo = GetTattooFromName("Full Black");

    public static Tattoo GetRandomTattoo()
    {
        var rarityRare = RareTattooAppearancePercent + PlayerPrefsHelper.GetBonusRarePercent();
        if (rarityRare > Constants.MaxRarePercent)
            rarityRare = Constants.MaxRarePercent;
        var rarityLegendary = LegendaryTattooAppearancePercent + PlayerPrefsHelper.GetBonusLegendaryPercent();
        if (rarityLegendary > Constants.MaxLegendaryPercent)
            rarityLegendary = Constants.MaxLegendaryPercent;

        int rarityPercent = UnityEngine.Random.Range(0, 100);
        if (rarityPercent < rarityLegendary)
            return GetRandomTattooFromRarity(Rarity.Legendary);
        else if (rarityPercent < rarityRare)
            return GetRandomTattooFromRarity(Rarity.Rare);
        else
            return GetRandomTattooFromRarity(Rarity.Common);
    }

    public static Tattoo GetRandomTattooFromRarity(Rarity rarity)
    {
        if (rarity == Rarity.Legendary)
            return GetTattooFromNames(LegendaryTattoosNames, UnityEngine.Random.Range(0, LegendaryTattoosNames.Length));
        else if (rarity == Rarity.Rare)
            return GetTattooFromNames(RareTattoosNames, UnityEngine.Random.Range(0, RareTattoosNames.Length));
        else
            return GetTattooFromNames(CommonTattoosNames, UnityEngine.Random.Range(0, CommonTattoosNames.Length));
    }

    public static Tattoo GetTattooFromNames(string[] names, int id, int loopId = -1)
    {
        if (id >= names.Length)
            id = 0;
        if (string.IsNullOrEmpty(names[id]))
            return null;
        var cleanName = names[id].Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        cleanName = cleanName.Replace("-", "");
        var instance = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + cleanName));
        if (PlayerPrefsHelper.GetMaxedOutTattoos().Contains((instance).Id) && loopId != id)
        {
            if (loopId == -1)
                loopId = id;
            return GetTattooFromNames(names, ++id, loopId);
        }
        return instance;
    }

    public static Tattoo GetTattooFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        var cleanName = name.Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        cleanName = cleanName.Replace("-", "");
        var instance = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + cleanName));
        return instance;
    }
}
