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
        var first = StepType.S0000;
        bool isOk = false;
        while (!isOk)
        {
            first = (StepType)Random.Range(3, Helper.EnumCount<StepType>());
            if (first != StepType.S0001
                && first != StepType.S0010
                && first != StepType.S0100
                && first != StepType.S1000)
                isOk = true;
        }
        if (character.StairwayToHeaven)
            first = StepType.S1000;
        var originStep = new Step(50, 50, run.CurrentRealm, first, true, false, LootType.None, 0, null);
        run.Steps += originStep.ToParsedString();
        GenerateAdjacentSteps(run, character, originStep, character.MapAquired ? run.MaxSteps : -1);
    }

    private string _adjacentString;

    public void GenerateAdjacentSteps(Run run, Character character, Step currentStep, int recursiveIteration = -1)
    {
        if (currentStep == null)
            return;
        var directionStr = currentStep.StepType.ToString().Substring(1);//SubString because StepType starts with a 'S'
        _adjacentString = string.Empty;
        for (int i = 0; i < 4; ++i)
        {
            if (directionStr[i] == '0')
                continue;
            var x = (i == 0 || i == 2) ? currentStep.X : (i == 1 ? currentStep.X + 1 : currentStep.X - 1);
            var y = (i == 1 || i == 3) ? currentStep.Y : (i == 0 ? currentStep.Y + 1 : currentStep.Y - 1);
            if (!TryGetStepOnPos(x, y, run.Steps, out var generatedStep))
                generatedStep = GenerateRandomStepAtPosition(x, y, run, character,
                    canEncounterCharacter: PlayerPrefsHelper.GetUnlockedCharactersString().Substring((int)run.CurrentRealm * 4, 4) != "1111",
                    customChancePercentToHaveAnExit: recursiveIteration == 0 ? 0 : -1);
            if (recursiveIteration > 0)
                GenerateAdjacentSteps(run, character, generatedStep, recursiveIteration - 1);
        }
    }

    public Step GenerateRandomStepAtPosition(int stepX, int stepY, Run run, Character character, bool canEncounterCharacter, int customChancePercentToHaveAnExit = -1)
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
        var chancePercentToHaveAnExit = customChancePercentToHaveAnExit >= 0 ? customChancePercentToHaveAnExit : 80;
        if (character.StairwayToHeaven && customChancePercentToHaveAnExit != 0)
            chancePercentToHaveAnExit = 100;
        for (int i = 0; i < 4; ++i)
        {
            if (chancePercentToHaveAnExit <= 0 || minimumExit[i] == '1' || minimumExit[i] == '.')
                continue;
            if (Helper.RandomDice100(chancePercentToHaveAnExit))
            {
                minimumExit = minimumExit.ReplaceChar(i, '1');
                if (!character.StairwayToHeaven)
                    chancePercentToHaveAnExit -= 20;
                else
                    break;
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
        var lootId = -1;
        var opponentType = OpponentType.Common;
        if (canEncounterCharacter && Helper.RandomDice100(run.GetCharEncounterPercent()) && !run.Steps.Contains("C") && run.CharacterEncounterAvailability && (run.CurrentRealm > Realm.Hell || run.RealmLevel > 1))
        {
            lootType = LootType.Character;
            var unlockedRealmString = PlayerPrefsHelper.GetUnlockedCharactersString().Substring((int)run.CurrentRealm * 4, 4);
            var unlockedIds = new List<int>();
            for (int i = 0; i < unlockedRealmString.Length; ++i)
            {
                if (unlockedRealmString[i] == '0')
                    unlockedIds.Add(i + (int)run.CurrentRealm * 4);
            }
            if (unlockedIds.Count == 0)
            {
                GenerateRandomStepAtPosition(stepX, stepY, run, character, canEncounterCharacter: false, customChancePercentToHaveAnExit);
                return null;
            }
            lootId = unlockedIds[Random.Range(0, unlockedIds.Count)];
            opponentType = OpponentType.Champion;
            PlayerPrefsHelper.ResetNumberRunWithoutCharacterEncounter(0);
            run.CharacterEncounterAvailability = false;
        }
        else if (Helper.RandomDice100(run.ItemLootPercent))
        {
            lootType = LootType.Item;
            var nbTries = 0;
            var currentItem = PlayerPrefsHelper.GetCurrentItemName();
            do
            {
                lootId = ItemsData.GetRandomItem().Id;
                ++nbTries;
            } while ((ItemsData.Items[lootId] == currentItem || run.Steps.Contains($"I{lootId.ToString("00")}")) && nbTries < 10);
            opponentType = (OpponentType)((int)((Item)Helper.GetLootFromTypeAndId(lootType, lootId)).Rarity);
        }
        else if (Helper.RandomDice100(run.ResourceLootPercent))
        {
            lootType = LootType.Resource;
            lootId = (int)run.CurrentRealm;
            opponentType = OpponentType.Common;
        }
        else if (Helper.RandomDice100(run.PactLootPercent))
        {
            lootType = LootType.Pact;
            var nbTries = 0;
            var currentPacts = PlayerPrefsHelper.GetCurrentPacts();
            do
            {
                lootId = PactsData.GetRandomPact().Id;
                ++nbTries;
            } while ((currentPacts.Any(p => p.Id == lootId) || run.Steps.Contains($"P{lootId.ToString("00")}")) && nbTries < 10);
            opponentType = (OpponentType)((int)((Pact)Helper.GetLootFromTypeAndId(lootType, lootId)).Rarity);
        }
        else
        {
            lootType = LootType.Tattoo;
            if (Mock.GetString(Constants.PpCurrentBodyParts).Length < Constants.AvailableBodyPartsIds.Length)
            {
                var loop = -1;
                while (loop < 1)
                {
                    lootId = TattoosData.GetRandomTattoo().Id;
                    if (!_adjacentString.Contains($"T{lootId.ToString("00")}"))
                        loop = 10;
                    --loop;
                    if (loop < 30)
                        loop = 10; //Just in case of an infinite loop
                }
            }
            else
            {
                lootId = -1;
                var firstRandomId = Random.Range(0, 12);
                int id = firstRandomId;
                var loop = -1;
                while (loop < 1)
                {
                    var alreadyTattoo = PlayerPrefsHelper.GetCurrentTattoos()[id];
                    if (alreadyTattoo.Level < alreadyTattoo.MaxLevel)
                    {
                        lootId = alreadyTattoo.Id;
                        break;
                    }
                    if (id == firstRandomId)
                        ++loop;
                    if (++id >= 12)
                        id = 0;
                }
            }
            if (lootId >= 0)
                opponentType = (OpponentType)((int)((Tattoo)Helper.GetLootFromTypeAndId(lootType, lootId)).Rarity);
            else
            {
                lootType = LootType.Resource;
                lootId = (int)run.CurrentRealm;
                opponentType = OpponentType.Common;
            }
        }
        //DEBUG
        if (ItemsData.DebugEnabled && !run.Steps.Contains($"I{ItemsData.DebugItem.Id.ToString("00")}"))
        {
            lootType = LootType.Item;
            lootId = ItemsData.DebugItem.Id;
            opponentType = (OpponentType)((int)((Item)Helper.GetLootFromTypeAndId(lootType, lootId)).Rarity);
        }
        if (TattoosData.DebugEnabled && (TattoosData.DebugMultitude || !run.Steps.Contains($"T{TattoosData.DebugTattoo.Id.ToString("00")}")))
        {
            lootType = LootType.Tattoo;
            lootId = TattoosData.DebugTattoo.Id;
            opponentType = (OpponentType)((int)((Tattoo)Helper.GetLootFromTypeAndId(lootType, lootId)).Rarity);
        }
        if (PactsData.DebugEnabled && (PactsData.DebugMultitude || !run.Steps.Contains($"P{PactsData.DebugPact.Id.ToString("00")}")))
        {
            lootType = LootType.Pact;
            lootId = PactsData.DebugPact.Id;
            opponentType = (OpponentType)((int)((Pact)Helper.GetLootFromTypeAndId(lootType, lootId)).Rarity);
        }
        if (CharactersData.DebugEnabled && !run.Steps.Contains($"C{CharactersData.DebugCharacter().Id.ToString("00")}"))
        {
            lootType = LootType.Character;
            lootId = CharactersData.DebugCharacter().Id;
            opponentType = OpponentType.Champion;
        }
        if (ResourcesData.DebugEnabled)
        {
            lootType = LootType.Resource;
            lootId = ResourcesData.DebugResource.Id;
            opponentType = OpponentType.Common;
        }
        //DEBUG
        if (character.SocialPyramid && opponentType == OpponentType.Champion)
            opponentType = OpponentType.Elite;
        var levelWeight = GetWeightFromRunLevel(run, character);
        var weight = Mathf.RoundToInt(levelWeight + (levelWeight * (0.2f * (int)opponentType)));
        var newStep = new Step(stepX, stepY, run.CurrentRealm, stepType, false, false, lootType, lootId, GetOpponentsFromWeight(run.CurrentRealm, weight, opponentType));
        _adjacentString += newStep.ToParsedString();
        run.Steps += newStep.ToParsedString();
        return newStep;
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

    public bool TryGetStepOnPos(int x, int y, string parsedStepsString, out Step outStep)
    {
        outStep = GetStepOnPos(x, y, parsedStepsString);
        return outStep != null;
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
        var unvisionnedSteps = steps.FindAll(s => !s.LandLordVision && s.LootType != LootType.None);
        if (unvisionnedSteps == null || unvisionnedSteps.Count == 0)
            return;
        var randomId = Random.Range(0, unvisionnedSteps.Count);
        SetLandLordVisionOnPos(unvisionnedSteps[randomId].X, unvisionnedSteps[randomId].Y, run);
    }

    public void SetVisionOnAllSteps(Run run)
    {
        var steps = GetAllSteps(run);
        var unvisionnedSteps = steps.FindAll(s => !s.LandLordVision && s.LootType != LootType.None);
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

    private List<Opponent> GetOpponentsFromWeight(Realm realm, int weight, OpponentType opponentType)
    {
        //Debug.Log($"\t[DEBUG]\tweight = {weight}");
        var realmOpponents = new List<Opponent>();
        var plausibleOpponents = new List<Opponent>();
        var stepOpponents = new List<Opponent>();
        if (realm == Realm.Hell)
            realmOpponents = OpponentsData.HellOpponents;
        else if (realm == Realm.Earth)
            realmOpponents = OpponentsData.EarthOpponents;
        else if (realm == Realm.Heaven)
            realmOpponents = OpponentsData.HeavenOpponents;
        int i = 0;
        var totalStepWeight = 0;
        //DEBUG
        if (OpponentsData.DebugEnabled)
        {
            stepOpponents.Add(OpponentsData.DebugOpponent());
            if (OpponentsData.OnlyOpponent)
                return stepOpponents;
        }
        //DEBUG
        plausibleOpponents = realmOpponents.FindAll(o => o.Weight > 0 && o.Weight <= weight && (int)o.Type <= (int)OpponentType.Boss);
        var averageWeight = plausibleOpponents.Sum(c => c.Weight) / plausibleOpponents.Count;
        var minSingleOpponentWeight = Random.Range(0, 3) == 0 ? 0 : averageWeight;
        while (i <= 12)
        {
            plausibleOpponents = realmOpponents.FindAll(o => o.Weight > minSingleOpponentWeight && o.Weight <= weight - totalStepWeight && (int)o.Type <= (int)opponentType);
            if (Cache.AlreadyConfrontedOpponents != null && Cache.AlreadyConfrontedOpponents.Count > 0)
            {
                var nonDoublonsPlausibleOpponents = plausibleOpponents.Where(o => !Cache.AlreadyConfrontedOpponents.Contains(o.Id)).ToList();
                if (nonDoublonsPlausibleOpponents.Count > 0)
                    plausibleOpponents = nonDoublonsPlausibleOpponents;
            }
            if (totalStepWeight < weight && plausibleOpponents != null && plausibleOpponents.Count > 0)
            {
                var opponent = plausibleOpponents[Random.Range(0, plausibleOpponents.Count)];
                if ((int)opponent.Type < opponentType.GetHashCode())
                {
                    opponent = Helper.UpgradeOpponentToUpperType(opponent, opponentType);
                    stepOpponents.Add(opponent);
                }
                else
                    stepOpponents.Add(opponent.Clone());
                Cache.AlreadyConfrontedOpponents ??= new List<int>();
                Cache.AlreadyConfrontedOpponents.Add(opponent.Id);
                totalStepWeight += opponent.Weight;
            }
            else
                i = 12;
            minSingleOpponentWeight = 0;
            ++i;
        }
        var difficulty = PlayerPrefsHelper.GetDifficulty();
        Helper.ApplyDifficulty(stepOpponents, difficulty);
        return stepOpponents;
    }

    private int GetWeightFromRunLevel(Run run, Character character)
    {
        var baseWeight = 50 + character.StepsWeightMalus;
        float weight = baseWeight;
        weight += (baseWeight * 0.75f) * (run.RealmLevel - 1);
        weight *= 1.0f + (0.30f * (int)run.CurrentRealm);
        if (run.Difficulty >= Difficulty.Hard)
        {
            var harderCount = ((int)run.Difficulty - Difficulty.Hard.GetHashCode()) + 1;
            weight *= Helper.MultiplierFromPercent(1.0f, Constants.WeightIncrementPercentPerDifficulty * harderCount);
        }
        return Mathf.RoundToInt(weight);
    }

    public Step GetClosestAvailableStepFromPos(int stepX, int stepY, Run run)
    {
        for (int i = 0; i < 4; ++i)
        {
            var x = (i == 0 || i == 2) ? stepX : (i == 1 ? stepX + 1 : stepX - 1);
            var y = (i == 1 || i == 3) ? stepY : (i == 0 ? stepY + 1 : stepY - 1);
            var tmpStep = GetStepOnPos(x, y, run.Steps);
            if (tmpStep != null && tmpStep.Discovered)
                return tmpStep;
        }
        return null;
    }

    public List<Opponent> GetBoss(Run run)
    {
        var realmOpponents = new List<Opponent>();
        if (run.CurrentRealm == Realm.Hell)
            realmOpponents = OpponentsData.HellOpponents;
        else if (run.CurrentRealm == Realm.Earth)
            realmOpponents = OpponentsData.EarthOpponents;
        else if (run.CurrentRealm == Realm.Heaven)
            realmOpponents = OpponentsData.HeavenOpponents;

        if (Cache.CurrentBossId != 0)
            return new List<Opponent>() { realmOpponents[Cache.CurrentBossId].Clone() };

        var idFromEnd = 1;
        var lastRandomBosses = PlayerPrefsHelper.GetLastRandomBosses();
        if (run.RealmLevel == 1)
        {
            if (lastRandomBosses[0] == lastRandomBosses[1])
            {
                if (lastRandomBosses[0] == '2')
                    idFromEnd = 3;
                else if (lastRandomBosses[0] == '3')
                    idFromEnd = 2;
                else
                    idFromEnd = Random.Range(2, 4);
            }
            else
                idFromEnd = Random.Range(2, 4);
            PlayerPrefsHelper.SetLastRandomBosses($"{lastRandomBosses[1]}{idFromEnd}");
        }
        var idBoss = realmOpponents.Count - idFromEnd;
        Cache.CurrentBossId = idBoss; 
        return new List<Opponent>() { realmOpponents[idBoss].Clone() };
    }
}
