using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TattoosData
{
    public static int LegendaryTattooAppearancePercent = 10;
    public static int RareTattooAppearancePercent = 30;

    public static string[] Tattoos = { "Tribal", "Trinity", "Cleaver", "Broken Clock" };

    public static string[] CommonTattoosNames = { Tattoos[0], Tattoos[1] };
    public static string[] RareTattoosNames = { Tattoos[2] };
    public static string[] LegendaryTattoosNames = { Tattoos[3] };

    public static Tattoo GetRandomTattoo()
    {
        int rarityPercent = UnityEngine.Random.Range(0, 100);
        if (rarityPercent < LegendaryTattooAppearancePercent)
            return GetRandomTattooFromRarity(Rarity.Legendary);
        else if (rarityPercent < RareTattooAppearancePercent)
            return GetRandomTattooFromRarity(Rarity.Rare);
        return GetRandomTattooFromRarity(Rarity.Common);
    }

    public static Tattoo GetRandomTattooFromRarity(Rarity rarity)
    {
        if (rarity == Rarity.Legendary)
            return GetTattooFromName(LegendaryTattoosNames[UnityEngine.Random.Range(0, LegendaryTattoosNames.Length)]);
        else if (rarity == Rarity.Rare)
            return GetTattooFromName(RareTattoosNames[UnityEngine.Random.Range(0, RareTattoosNames.Length)]);
        return GetTattooFromName(CommonTattoosNames[UnityEngine.Random.Range(0, CommonTattoosNames.Length)]);
    }

    public static Tattoo GetTattooFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        var cleanName = name.Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        var instance = Activator.CreateInstance(Type.GetType("Tattoo" + cleanName));
        return (Tattoo)instance;
    }
}
