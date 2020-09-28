using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StepsService
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
                else if (i == 0)
                    alreadyExitIdToCheck = 2;
                else if (i == 1)
                    alreadyExitIdToCheck = 3;
                if (alreadyStep.StepType.ToString().Substring(1)[alreadyExitIdToCheck] == '1')
                    minimumExit = minimumExit.ReplaceChar(i, '1');
                else
                    minimumExit = minimumExit.ReplaceChar(i, '.');
            }
        }
        var chancePercentageToHaveAnExit = 70;
        for (int i = 0; i < 4; ++i)
        {
            if (minimumExit[i] == '1' || minimumExit[i] == '.')
                continue;
            if (Helper.RandomDice100(chancePercentageToHaveAnExit))
            {
                minimumExit = minimumExit.ReplaceChar(i, '1');
                chancePercentageToHaveAnExit -= 20;
            }
        }
        minimumExit = minimumExit.Replace('.', '0');
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
        var opponentType = OpponentType.Common;
        if (run.CharacterEncounterAvailability && Helper.RandomDice100(run.CharEncounterPercent))
        {
            lootType = LootType.Character;
            opponentType = OpponentType.Common;
        }
        else if (Helper.RandomDice100(run.ItemLootPercent))
        {
            lootType = LootType.Item;
            lootId = ItemsData.GetRandomItem().Id;
            opponentType = (OpponentType)((Item)Helper.GetLootFromTypeAndId(lootType, lootId)).Rarity.GetHashCode();
        }
        else
        {
            lootType = LootType.Tattoo;
            if (PlayerPrefs.GetString(Constants.PpCurrentBodyParts).Length < Constants.AvailableBodyPartsIds.Length)
                lootId = TattoosData.GetRandomTattoo().Id;
            else
                lootId = PlayerPrefsHelper.GetCurrentTattoos()[Random.Range(0, 12)].Id;
            opponentType = (OpponentType)((Tattoo)Helper.GetLootFromTypeAndId(lootType, lootId)).Rarity.GetHashCode();
        }
        var levelDifficulty = GetDifficultyWeightFromRunLevel(run);
        var difficulty = (int)(levelDifficulty + (levelDifficulty * (0.2f * opponentType.GetHashCode())));
        var newStep = new Step(stepX, stepY, run.CurrentRealm, stepType, false, false, lootType, lootId, GetOpponentsFromDifficultyWeight(run.CurrentRealm, difficulty, opponentType));
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

    public void DiscoverStepOnPos(int x, int y, Run run)
    {
        var stepStartId = run.Steps.IndexOf("X" + x.ToString("00") + "Y" + y.ToString("00"));
        if (stepStartId == -1)
            return;
        var stepDiscoveredValueId = run.Steps.Substring(stepStartId).IndexOf('D') + 1;
        run.Steps = run.Steps.ReplaceChar(stepStartId + stepDiscoveredValueId, '1');
    }

    public void ClearLootOnPos(int x, int y, Run run)
    {
        var stepStartId = run.Steps.IndexOf("X" + x.ToString("00") + "Y" + y.ToString("00"));
        if (stepStartId == -1)
            return;
        var stepLootTypeId = run.Steps.Substring(stepStartId).IndexOf('V') + 2;
        run.Steps = run.Steps.ReplaceChar(stepStartId + stepLootTypeId, 'N');
    }

    public void SetVisionOnRandomStep(Run run)
    {
        var steps = GetAllSteps(run);
        var unvisionnedSteps = steps.FindAll(s => !s.LandLordVision && !s.Discovered);
        if (unvisionnedSteps == null || unvisionnedSteps.Count == 0)
            return;
        var randomId = Random.Range(0, unvisionnedSteps.Count);
        SetLandLordVisionOnPos(unvisionnedSteps[randomId].X, unvisionnedSteps[randomId].Y, run);
    }

    public void SetVisionOnAllSteps(Run run)
    {
        var steps = GetAllSteps(run);
        var unvisionnedSteps = steps.FindAll(s => !s.LandLordVision && !s.Discovered);
        foreach (Step step in unvisionnedSteps)
            SetLandLordVisionOnPos(step.X, step.Y, run);
    }

    public void SetLandLordVisionOnPos(int x, int y, Run run)
    {
        var stepStartId = run.Steps.IndexOf("X" + x.ToString("00") + "Y" + y.ToString("00"));
        if (stepStartId == -1)
            return;
        var stepVisionValueId = run.Steps.Substring(stepStartId).IndexOf('V') + 1;
        run.Steps = run.Steps.ReplaceChar(stepStartId + stepVisionValueId, '1');
    }

    private List<Opponent> GetOpponentsFromDifficultyWeight(Realm realm, int difficultyWeight, OpponentType opponentType)
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
        plausibleOpponents = realmOpponents.FindAll(o => o.DifficultyWeight > 0 && o.DifficultyWeight <= difficultyWeight && o.Type.GetHashCode() <= opponentType.GetHashCode());
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
                var opponent = plausibleOpponents[Random.Range(0, plausibleOpponents.Count)];
                if (opponent.Type.GetHashCode() < opponentType.GetHashCode())
                {
                    var typeDifference = opponentType.GetHashCode() - opponent.Type.GetHashCode();
                    opponent.Cooldown -= (int)(opponent.Cooldown * (typeDifference * 0.2f));
                    opponent.HpMax += (int)(opponent.HpMax * (typeDifference * 0.2f));
                    opponent.Type = opponentType;
                }
                stepOpponents.Add(opponent);
                plausibleOpponents = realmOpponents.FindAll(o => o.DifficultyWeight > 0 && o.DifficultyWeight <= difficultyWeight - totalStepWeight);
            }
            else
                i = 12;
            ++i;
        }
        return stepOpponents;
    }

    private int GetDifficultyWeightFromRunLevel(Run run)
    {
        var baseWeight = 40;
        var difficultyWeight = baseWeight;
        difficultyWeight += (int)((baseWeight * 0.75f) * (run.RealmLevel - 1));
        difficultyWeight *= (int)(1.50f * (run.CurrentRealm.GetHashCode() + 1));
        return difficultyWeight;
    }

    public Step GetClosestAvailableStepFromPos(int stepX, int stepY, Run run)
    {
        for (int i = 0; i < 4; ++i)
        {
            var x = (i == 0 || i == 2) ? stepX : (i == 1 ? stepX + 1 : stepX - 1);
            var y = (i == 1 || i == 3) ? stepY : (i == 0 ? stepY + 1 : stepY - 1);
            var tmpStep = GetStepOnPos(x, y, run.Steps);
            if (tmpStep.Discovered)
                return tmpStep;
        }
        return null;
    }
}
