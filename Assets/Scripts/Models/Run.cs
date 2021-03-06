﻿using System.Collections.Generic;

public class Run
{
    public Realm CurrentRealm;
    public Difficulty Difficulty;
    public int RealmLevel;
    public int MaxSteps;
    public int CurrentStep;
    public int CurrentItemCooldown;
    public int CurrentItemUses;
    public string ResourcesLooted;
    public string Steps;
    public int X, Y;
    public bool IsEndless;
    // Character starts at X50 Y50 because coordinates are stored in two digits and coordinates under zero could mean up to 3 digits
    // X = X
    // Y = Y
    // R = Realm
    // S = Step Type ID
    // D = Discovered 0/1
    // V LandLordVision 0/1
    // I = Item ID (-1 = none) || T = Tattoo ID (-1 = none) || C = Character ID (-1 = none)
    // O = Realm Opponents IDs
    // ; = Step End
    // Example (without spaces) = X00 Y00 R0 T00 D0 V0 I00 O00 01 02 03 04 05 06 07 08 ;
    // 1110

    public bool CharacterEncounterAvailability = true;
    public int CharEncounterPercent = 5;
    public int ItemLootPercent = 20;
    public int ResourceLootPercent = 20;

    public int DeathScytheAscension = 0;
    public bool LifeRouletteOnce = false;
    public bool RepentanceOnce = false;

    public Run(Difficulty difficulty)
    {
        Difficulty = difficulty;
        CurrentRealm = Realm.Hell;
        RealmLevel = 0;
        IncreaseLevel();
    }

    public int GetCharEncounterPercent()
    {
        var bonusPercent = PlayerPrefsHelper.GetNumberRunWithoutCharacterEncounter() * 5;
        return CharEncounterPercent + bonusPercent;
    }
        

    public void IncreaseLevel()
    {
        ++RealmLevel;
        if (RealmLevel > 3)
        {
            RealmLevel = 1;
            if (CurrentRealm == Realm.Hell)
                CurrentRealm = Realm.Earth;
            else if (CurrentRealm == Realm.Earth)
                CurrentRealm = Realm.Heaven;
        }
        MaxSteps = 5;
        if (Difficulty == Difficulty.Easy)
            MaxSteps = 6;
        else if (Difficulty == Difficulty.Hard)
            MaxSteps = 4;
        else if (Difficulty == Difficulty.Infernal)
            MaxSteps = 3;
        var realmTree = PlayerPrefsHelper.GetRealmTree();
        MaxSteps += realmTree.Shadowing;
        CurrentStep = 0;
        X = 50;
        Y = 50;
        Steps = null;
    }

    public List<int> GetRunResources()
    {
        var resourcesStr = ResourcesLooted;
        var resources = new List<int>();
        if (!string.IsNullOrEmpty(resourcesStr))
        {
            var resourcesStrSplit = resourcesStr.Split(';');
            for (int i = 0; i < resourcesStrSplit.Length; ++i)
            {
                if (!string.IsNullOrEmpty(resourcesStrSplit[i]))
                    resources.Add(int.Parse(resourcesStrSplit[i]));
            }
        }
        else
        {
            for (int i = 0; i < ResourcesData.Resources.Length; ++i)
                resources.Add(0);
        }
        return resources;
    }

    public void AlterResource(int resourceId, int amount)
    {
        var resources = GetRunResources();
        resources[resourceId] += amount;
        var resourcesStr = "";
        foreach (int resource in resources)
        {
            resourcesStr += $"{resource};";
        }
        ResourcesLooted = resourcesStr;
    }
}
