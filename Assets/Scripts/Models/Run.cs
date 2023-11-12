using System.Collections.Generic;
using UnityEngine;

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
    public int Endless;
    public bool DebugEnabled = Constants.RunDebug;
    // Character starts at X50 Y50 because coordinates are stored in two digits and coordinates under zero could mean up to 3 digits
    // X = X
    // Y = Y
    // R = Realm
    // S = Step Type ID
    // D = Discovered 0/1
    // V LandLordVision 0/1
    // I = Item ID (-1 = none) || T = Tattoo ID (-1 = none) || C = Character ID (-1 = none) || R = Resource || P = Pact ID
    // O = Realm Opponents IDs
    // ; = Step End
    // Example (without spaces) = X00 Y00 R0 T00 D0 V0 I00 O00 01 02 03 04 05 06 07 08 ;
    // 1110

    public bool CharacterEncounterAvailability = true;
    public int CharEncounterPercent = 5;
    public int ItemLootPercent = 15;
    public int ResourceLootPercent = 20;
    public int PactLootPercent = 20;
    public int TattooLootPercent = 40;

    public int DeathScytheCount;
    public bool LifeRouletteOnce;
    public bool RepentanceOnce;

    public Run(Difficulty difficulty)
    {
        Difficulty = difficulty;
        CurrentRealm = DebugEnabled ? Realm.Earth : Realm.Hell;
        RealmLevel = 0;
        IncreaseLevel();
    }

    public int GetCharEncounterPercent()
    {
        var bonusPercent = PlayerPrefsHelper.GetNumberRunWithoutCharacterEncounter() * 10;
        if (bonusPercent > 50)
            bonusPercent = 50;
        return CharEncounterPercent + bonusPercent;
    }
        

    public void IncreaseLevel(Character character = null)
    {
        ++RealmLevel;
        if (RealmLevel > 2)
        {            
            RealmLevel = 1;
            if (CurrentRealm == Realm.Hell)
                CurrentRealm = Realm.Earth;
            else if (CurrentRealm == Realm.Earth)
                CurrentRealm = Realm.Heaven;
            else if (CurrentRealm == Realm.Heaven)
                CurrentRealm = Realm.End;
            if (CharacterEncounterAvailability)
                PlayerPrefsHelper.IncrementNumberRunWithoutCharacterEncounter();
            else
                CharacterEncounterAvailability = true;
            Cache.AlreadyConfrontedOpponents = null;
        }
        MaxSteps = 4;
        if (Difficulty == Difficulty.Easy)
            MaxSteps = 5;
        else if (Difficulty == Difficulty.Hard)
            MaxSteps = 3;
        else if (Difficulty == Difficulty.Infernal)
            MaxSteps = 2;
        else if (Difficulty >= Difficulty.Divine)
            MaxSteps = 1;
        var realmTree = PlayerPrefsHelper.GetRealmTree();
        if (character != null)
            MaxSteps += Mathf.RoundToInt(realmTree.Shadowing * Helper.MultiplierFromPercent(1.0f, character.RealmTreeBoost));
        else
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
