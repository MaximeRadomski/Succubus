﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PactsData
{
    public static int LegendaryPactAppearancePercent = 1;
    public static int RarePactAppearancePercent = 15;

    public static string[] Pacts =
    /* 0  */ { "Devoted Apostle", "Failed Baptism", "Hellish Idolatry", "Small Paradise", "Divine Resurection", "False Testimony", "Polarised Temperance", "Righteous Sacrifice", "Short Rapture", "Cursed Prophet",
    /* 10 */   "Lusty Patriarch", "Envious Messiah", "Instant Epiphany", "Altar of Purity", "Dishonest Sermon", "Pleasant Tribulation", "Unexpected Revelation", "Sinful Divinity", "Arousing Disguise", "Reprisable Justice" };

    //list https://www.vocabulary.com/lists/185056

    public static string[] CommonPactsNames =    { Pacts[00], Pacts[03], Pacts[06], Pacts[08], Pacts[10], Pacts[11], Pacts[12], Pacts[15], Pacts[16], Pacts[18], Pacts[19] };
    public static string[] RarePactsNames =      { Pacts[01], Pacts[04], Pacts[07], Pacts[09], Pacts[14], Pacts[17] };
    public static string[] LegendaryPactsNames = { Pacts[02], Pacts[05], Pacts[13] };

    // DEBUG //
    public static bool DebugEnabled = Constants.PactsDebug;
    public static bool DebugMultitude = false;
    public static Pact DebugPact = GetPactFromName("Small Paradise");

    public static Pact GetRandomPact()
    {
        var rareBonus = PlayerPrefsHelper.GetBonusRarePercent();
        if (rareBonus > Constants.MaxRarePercent)
            rareBonus = Constants.MaxRarePercent;
        var rarityRare = RarePactAppearancePercent + rareBonus;

        var legendaryBonus = PlayerPrefsHelper.GetBonusLegendaryPercent();
        if (legendaryBonus > Constants.MaxLegendaryPercent)
            legendaryBonus = Constants.MaxLegendaryPercent;
        var rarityLegendary = LegendaryPactAppearancePercent + legendaryBonus;

        int rarityPercent = UnityEngine.Random.Range(0, 100);
        if (rarityPercent < rarityLegendary)
            return GetRandomPactFromRarity(Rarity.Legendary);
        else if (rarityPercent < rarityRare)
            return GetRandomPactFromRarity(Rarity.Rare);
        else
            return GetRandomPactFromRarity(Rarity.Common);
    }

    public static Pact GetRandomPactFromRarity(Rarity rarity)
    {
        if (rarity == Rarity.Legendary)
            return GetPactFromNames(LegendaryPactsNames, UnityEngine.Random.Range(0, LegendaryPactsNames.Length));
        else if (rarity == Rarity.Rare)
            return GetPactFromNames(RarePactsNames, UnityEngine.Random.Range(0, RarePactsNames.Length));
        else
            return GetPactFromNames(CommonPactsNames, UnityEngine.Random.Range(0, CommonPactsNames.Length));
    }

    public static Pact GetPactFromNames(string[] names, int id, int loopId = -1)
    {
        if (id >= names.Length)
            id = 0;
        if (string.IsNullOrEmpty(names[id]))
            return null;
        var cleanName = names[id].Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        cleanName = cleanName.Replace("-", "");
        var instance = (Pact)Activator.CreateInstance(Type.GetType("Pact" + cleanName, true));
        return instance;
    }

    public static Pact GetPactFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        var cleanName = name.Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        cleanName = cleanName.Replace("-", "");
        var instance = (Pact)Activator.CreateInstance(Type.GetType("Pact" + cleanName));
        return instance;
    }
}
