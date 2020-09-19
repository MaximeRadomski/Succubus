using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StepService
{
    public List<Step> GetAllSteps(Run run)
    {
        int start = -1;
        int end = -1;
        var listSteps = new List<Step>();
        for (int i = 0; i < run.Steps.Length; ++i)
        {
            if (i == 0 || run.Steps[i - 1] == ';')
                start = i;
            else if (run.Steps[i] == ';')
            {
                end = i;
                listSteps.Add(new Step(run.Steps.Substring(start, (end - start) + 1)));
            }
        }
        return listSteps;
    }

    public void GenerateOriginSteps(Run run, Character character)
    {
        run.Steps = "";
        var originStep = new Step(50, 50, run.CurrentRealm, StepType.S1111, true, false, LootType.None, 0, null);
        run.Steps += originStep.ToParsedString();
        GenerateAdjacentSteps(run, character, originStep);
    }

    public void GenerateAdjacentSteps(Run run, Character character, Step currentStep)
    {
        var directionStr = currentStep.StepType.ToString().Substring(1);//SubString because StepType starts with a 'S'
        for (int i = 0; i < 4; ++i)
        {
            if (directionStr[i] == '0')
                continue;
            var x = (i == 0 || i == 2) ? currentStep.X : (i == 1 ? currentStep.X + 1 : currentStep.X - 1);
            var y = (i == 1 || i == 3) ? currentStep.Y : (i == 0 ? currentStep.Y + 1 : currentStep.Y - 1);
            if (GetStepOnPos(x, y, run.Steps) != null)
                continue;
            GenerateRandomStepAtPosition(x, y, run, character);
        }
    }

    public void GenerateRandomStepAtPosition(int stepX, int stepY, Run run, Character character)
    {
        var minimumExit = "0000";
        for (int i = 0; i < 4; ++i)
        {
            var x = (i == 0 || i == 2) ? stepX : (i == 1 ? stepX + 1 : stepX - 1);
            var y = (i == 1 || i == 3) ? stepY : (i == 0 ? stepY + 1 : stepY - 1);
            var alreadyStep = GetStepOnPos(x, y, run.Steps);
            if (alreadyStep != null)
            {
                var alreadyExitIdToCheck = 0;
                if (i == 3)
                    alreadyExitIdToCheck = 1;
                else if (i == 1)
                    alreadyExitIdToCheck = 2;
                else if (i == 3)
                    alreadyExitIdToCheck = 3;
                if (alreadyStep.StepType.ToString().Substring(1)[alreadyExitIdToCheck] == '1')
                    minimumExit = minimumExit.ReplaceChar(i, '1');
            }
        }
        var chancePercentageToHaveAnExit = 50;
        for (int i = 0; i < 4; ++i)
        {
            if (minimumExit[i] == '1')
                continue;
            if (Helper.RandomDice100(chancePercentageToHaveAnExit))
            {
                minimumExit = minimumExit.ReplaceChar(i, '1');
                chancePercentageToHaveAnExit -= chancePercentageToHaveAnExit / 2;
            }
        }
        var typeNames = System.Enum.GetNames(typeof(StepType)).ToList();
        var stepType = StepType.S0000;
        for (int i = 0; i < typeNames.Count; ++i)
        {
            if (typeNames[i].Substring(1) == minimumExit)
            {
                stepType = (StepType)i;
                break;
            }
        }
        var lootType = LootType.None;
        var lootId = 0;
        if (run.CharacterEncounterAvailability && Helper.RandomDice100(run.CharEncounterPercent))
        {
            lootType = LootType.Character;
        }
        else if (Helper.RandomDice100(run.ItemLootPercent))
        {
            lootType = LootType.Item;
            lootId = ItemsData.GetRandomItem().Id;
        }
        else
        {
            lootType = LootType.Tattoo;
            lootId = TattoosData.GetRandomTattoo().Id;
        }
        var newStep = new Step(stepX, stepY, run.CurrentRealm, stepType, false, false, lootType, lootId, GetOpponentsFromDifficultyWeight(run.CurrentRealm, GetDifficultyWieghtFromRunLevel(run)));
        run.Steps += newStep.ToParsedString();
    }

    public Step GetStepOnPos(int x, int y, string parsedStepsString)
    {
        var stepStartId = parsedStepsString.IndexOf("X" + x.ToString("00") + "Y" + y.ToString("00"));
        if (stepStartId == -1)
            return null;
        var stepEndId = parsedStepsString.Substring(stepStartId).IndexOf(';');
        var stepStr = parsedStepsString.Substring(stepStartId, stepEndId + 1);
        return new Step(stepStr);
    }

    private List<Opponent> GetOpponentsFromDifficultyWeight(Realm realm, int difficultyWeight)
    {
        var realmOpponents = new List<Opponent>();
        var plausibleOpponents = new List<Opponent>();
        var stepOpponents = new List<Opponent>();
        if (realm == Realm.Hell)
            realmOpponents = OpponentsData.HellOpponents;
        else if (realm == Realm.Earth)
            realmOpponents = OpponentsData.EarthOpponents;
        else if (realm == Realm.Heaven)
            realmOpponents = OpponentsData.HeavenOpponents;
        plausibleOpponents = realmOpponents.FindAll(o => o.DifficultyWeight > 0 && o.DifficultyWeight <= difficultyWeight);
        int i = 0;
        while (i <= 12)
        {
            var totalStepWeight = 0;
            foreach (var opponent in stepOpponents)
            {
                totalStepWeight += opponent.DifficultyWeight;
            }
            if (totalStepWeight < difficultyWeight)
            {
                stepOpponents.Add(plausibleOpponents[Random.Range(0, plausibleOpponents.Count)]);
                plausibleOpponents = realmOpponents.FindAll(o => o.DifficultyWeight > 0 && o.DifficultyWeight <= difficultyWeight - totalStepWeight);
            }
            else
                i = 12;
            ++i;
        }
        return stepOpponents;
    }

    private int GetDifficultyWieghtFromRunLevel(Run run)
    {
        var baseWeight = 40;
        var difficultyWeight = baseWeight;
        difficultyWeight += (int)(baseWeight * 0.75f) * run.RealmLevel;
        difficultyWeight *= run.CurrentRealm.GetHashCode() + 1;
        return difficultyWeight;
    }    
}
