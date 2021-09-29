using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerPrefsHelper : MonoBehaviour
{
    public static void SaveRun(Run run)
    {
        Mock.SetString(Constants.PpRun, JsonUtility.ToJson(run));
    }

    public static Run GetRun()
    {
        var run = JsonUtility.FromJson<Run>(Mock.GetString(Constants.PpRun, Constants.PpSerializeDefault));
        if (run == null)
            return null;
        return run;
    }

    public static void ResetRun()
    {
        SaveRun(null);
    }

    public static void EndlessRun(Run run)
    {
        ++run.Endless;
        run.CurrentRealm = Realm.Hell;
        run.RealmLevel = 1;
        if (run.Difficulty != Difficulty.Divine666)
            run.Difficulty = (Difficulty)(run.Difficulty.GetHashCode() + 1);
        SaveRun(run);
    }

    public static void SaveBag(string bag)
    {
        PlayerPrefs.SetString(Constants.PpBag, bag);
    }

    public static string GetBag()
    {
        var bag = PlayerPrefs.GetString(Constants.PpBag, Constants.PpSerializeDefault);
        return bag;
    }

    public static void SaveHolder(string holder)
    {
        PlayerPrefs.SetString(Constants.PpHolder, holder);
    }

    public static string GetHolder()
    {
        var holder = PlayerPrefs.GetString(Constants.PpHolder, Constants.PpSerializeDefault);
        return holder;
    }

    public static void SaveTraining(int score, int level, int lines, int pieces, List<float> verif)
    {
        Mock.SetInt(Constants.PpTrainingScore, score);
        Mock.SetInt(Constants.PpTrainingLevel, level);
        Mock.SetInt(Constants.PpTrainingLines, lines);
        Mock.SetInt(Constants.PpTrainingPieces, pieces);
        Mock.SetString(Constants.PpVerif, verif.ToParsedString(';'));
    }

    public static void ResetTraining()
    {
        Mock.SetInt(Constants.PpTrainingScore, 0);
        Mock.SetInt(Constants.PpTrainingLevel, 1);
        Mock.SetInt(Constants.PpTrainingLines, 0);
        Mock.SetInt(Constants.PpTrainingPieces, 0);
        Mock.SetString(Constants.PpVerif, null);
    }

    public static List<int> GetTraining()
    {
        var results = new List<int>();
        results.Add(Mock.GetInt(Constants.PpTrainingScore, 0));
        results.Add(Mock.GetInt(Constants.PpTrainingLevel, 1));
        results.Add(Mock.GetInt(Constants.PpTrainingLines, 0));
        results.Add(Mock.GetInt(Constants.PpTrainingPieces, 0));
        return results;
    }

    public static List<float> GetVerif()
    {
        var parsedVerif = Mock.GetString(Constants.PpVerif, Constants.PpSerializeDefault);
        if (string.IsNullOrEmpty(parsedVerif))
            return new List<float>() { 0.0f, 0.0f, 0.0f, 0.0f, 4.0f};
        var verifArray = parsedVerif.Split(';');
        var verifList = verifArray.ToFloatList();
        return verifList;
    }

    public static void SaveTrainingHighScoreHistory(List<int> scoreHistory, bool isOldSchool)
    {
        var scoreHistoryStr = "";
        foreach (var score in scoreHistory)
            scoreHistoryStr += $"{score};";
        Mock.SetString(isOldSchool ? Constants.PpTrainingHighScoreHistoryOldSchool : Constants.PpTrainingHighScoreHistory, scoreHistoryStr);
    }

    public static List<int> GetTrainingHighScoreHistory(bool isOldSchool)
    {
        var scoreHistoryStr = Mock.GetString(isOldSchool ? Constants.PpTrainingHighScoreHistoryOldSchool : Constants.PpTrainingHighScoreHistory, Constants.PpSerializeDefault);
        int i = 0;
        var scoreHistory = new List<int>();
        while (!string.IsNullOrEmpty(scoreHistoryStr) || i > 15)
        {
            var separatorScoreId = scoreHistoryStr.IndexOf(';');
            if (separatorScoreId == -1)
                break;
            var score = int.Parse(scoreHistoryStr.Substring(0, separatorScoreId));
            scoreHistory.Add(score);
            if (separatorScoreId + 1 >= scoreHistoryStr.Length)
                break;
            scoreHistoryStr = scoreHistoryStr.Substring(scoreHistoryStr.IndexOf(';') + 1);
            ++i;
        }
        return scoreHistory;
    }

    public static void SaveTrainingHighestScore(List<int> scoreContext, string encryptedScore, int type, bool isOldSchool)
    {
        var lastCredentials = PlayerPrefsHelper.GetLastSavedCredentials();
        var score = new HighScoreDto(lastCredentials != null && lastCredentials.PlayerName != null ? lastCredentials.PlayerName : "", scoreContext[0], scoreContext[1], scoreContext[2], scoreContext[3], scoreContext[4], type, encryptedScore);
        Mock.SetString(isOldSchool ? Constants.PpTrainingHighestScoreOldSchool : Constants.PpTrainingHighestScore, JsonUtility.ToJson(score));
    }

    public static HighScoreDto GetTrainingHighestScore(bool isOldSchool)
    {
        var score = JsonUtility.FromJson<HighScoreDto>(Mock.GetString(isOldSchool ? Constants.PpTrainingHighestScoreOldSchool : Constants.PpTrainingHighestScore, Constants.PpSerializeDefault));
        if (score == null)
            return null;
        return score;
    }

    public static void SaveGhostColor(string ghostColor)
    {
        PlayerPrefs.SetString(Constants.PpGhostPieceColor, ghostColor);
    }

    public static string GetGhostColor()
    {
        var ghostColor = PlayerPrefs.GetString(Constants.PpGhostPieceColor, Constants.PpGhostPieceColorDefault);
        return ghostColor;
    }

    public static void SaveGameplayChoice(GameplayChoice gameplayChoice)
    {
        PlayerPrefs.SetInt(Constants.PpGameplayChoice, gameplayChoice.GetHashCode());
    }

    public static GameplayChoice GetGameplayChoice()
    {
        var gameplayChoice = PlayerPrefs.GetInt(Constants.PpGameplayChoice, Constants.PpGameplayChoiceDefault);
        return (GameplayChoice)gameplayChoice;
    }

    public static void SaveButtonsLeftPanel(string buttons)
    {
        PlayerPrefs.SetString(Constants.PpButtonsLeftPanel, buttons);
    }

    public static string GetButtonsLeftPanel()
    {
        var buttons = PlayerPrefs.GetString(Constants.PpButtonsLeftPanel, Constants.PpButtonsLeftPanelDefault);
        return buttons;
    }

    public static void SaveButtonsRightPanel(string buttons)
    {
        PlayerPrefs.SetString(Constants.PpButtonsRightPanel, buttons);
    }

    public static string GetButtonsRightPanel()
    {
        var buttons = PlayerPrefs.GetString(Constants.PpButtonsRightPanel, Constants.PpButtonsRightPanelDefault);
        return buttons;
    }

    public static void SaveKeyBinding(List<KeyCode> keyBinding)
    {
        var strKeyBinding = "";
        foreach (var key in keyBinding)
            strKeyBinding += key.GetHashCode() + ";";
        PlayerPrefs.SetString(Constants.PpKeyBinding, strKeyBinding);
    }

    public static List<KeyCode> GetKeyBinding(string customStr = null)
    {
        string strKeyBinding;
        if (customStr == null)
            strKeyBinding = PlayerPrefs.GetString(Constants.PpKeyBinding, Constants.PpKeyBindingDefault);
        else
            strKeyBinding = customStr;
        var keyBindings = new List<KeyCode>();
        int i = 0;
        while (!string.IsNullOrEmpty(strKeyBinding) || i >= 16)
        {
            var separatorId = strKeyBinding.IndexOf(';');
            if (separatorId == -1)
                break;
            var keyCodeHashCode = int.Parse(strKeyBinding.Substring(0, separatorId));
            keyBindings.Add((KeyCode)keyCodeHashCode);
            if (separatorId + 1 >= strKeyBinding.Length)
                break;
            strKeyBinding = strKeyBinding.Substring(separatorId + 1);
            ++i;
        }
        if (keyBindings.Count < 16)
            keyBindings = GetKeyBinding(Constants.PpKeyBindingDefault);
        return keyBindings;
    }

    public static void AddUnlockedCharacters(Character character)
    {
        var currentUnlockedCharactersString = GetUnlockedCharactersString();
        currentUnlockedCharactersString = currentUnlockedCharactersString.ReplaceChar(character.Id, '1');
        SaveUnlockedCharacters(currentUnlockedCharactersString);
    }

    public static void SaveUnlockedCharacters(string unlockedCharacters)
    {
        Mock.SetString(Constants.PpUnlockedCharacters, unlockedCharacters);
    }

    public static string GetUnlockedCharactersString()
    {
        var unlockedCharacters = Mock.GetString(Constants.PpUnlockedCharacters, Constants.PpUnlockedCharactersDefault);
        return unlockedCharacters;
    }

    public static void SaveSelectedCharacter(int selectedCharacter)
    {
        Mock.SetInt(Constants.PpSelectedCharacter, selectedCharacter);
    }

    public static int GetSelectedCharacterId()
    {
        var selectedCharacter = Mock.GetInt(Constants.PpSelectedCharacter, Constants.PpSelectedCharacterDefault);
        return selectedCharacter;
    }

    public static void SaveRunCharacter(Character character)
    {
        Mock.SetString(Constants.PpRunCharacter, JsonUtility.ToJson(character));
    }

    public static Character GetRunCharacter()
    {
        var character = JsonUtility.FromJson<Character>(Mock.GetString(Constants.PpRunCharacter, Constants.PpSerializeDefault));
        if (character == null)
            return CharactersData.Characters[GetSelectedCharacterId()];
        return character;
    }

    public static void SaveDifficulty(Difficulty difficulty)
    {
        PlayerPrefs.SetInt(Constants.PpDifficulty, difficulty.GetHashCode());
    }

    public static Difficulty GetDifficulty()
    {
        var difficulty = PlayerPrefs.GetInt(Constants.PpDifficulty, Constants.PpDifficultyDefault);
        return (Difficulty)difficulty;
    }

    public static void SaveCurrentOpponents(List<Opponent> opponents)
    {
        if (opponents == null)
        {
            Mock.SetString(Constants.PpCurrentOpponents, Constants.PpSerializeDefault);
            return;
        }
        var opponentsStr = "";
        foreach (var opponent in opponents)
        {
            opponentsStr += opponent.Realm.GetHashCode() + ":" + opponent.Id + ";";
        }
        Mock.SetString(Constants.PpCurrentOpponents, opponentsStr);
    }

    public static List<Opponent> GetCurrentOpponents(Run run)
    {
        var opponentsStr = Mock.GetString(Constants.PpCurrentOpponents, Constants.PpSerializeDefault);
        int i = 0;
        var opponentsList = new List<Opponent>();
        while (!string.IsNullOrEmpty(opponentsStr) || i > 15)
        {
            var separatorRealmId = opponentsStr.IndexOf(':');
            var separatorId = opponentsStr.IndexOf(';');
            if (separatorId == -1)
                break;
            var realmId = int.Parse(opponentsStr.Substring(0, separatorRealmId));
            var tmpId = int.Parse(opponentsStr.Substring(separatorRealmId + 1, separatorId - (separatorRealmId + 1)));
            Opponent tmpOpponent = null;
            Realm opponentsRealm = run?.CurrentRealm ?? (Realm)realmId;
            if (OpponentsData.DebugEnabled)
                opponentsRealm = OpponentsData.DebugRealm;
            if (opponentsRealm == Realm.Hell)
                tmpOpponent = OpponentsData.HellOpponents[tmpId];
            else if (opponentsRealm == Realm.Earth)
                tmpOpponent = OpponentsData.EarthOpponents[tmpId];
            else if (opponentsRealm == Realm.Heaven)
                tmpOpponent = OpponentsData.HeavenOpponents[tmpId];
            opponentsList.Add(tmpOpponent.Clone());
            if (separatorId + 1 >= opponentsStr.Length)
                break;
            opponentsStr = opponentsStr.Substring(separatorId + 1);
            ++i;
        }
        return opponentsList;
    }

    public static void ResetCurrentItem()
    {
        Mock.SetString(Constants.PpCurrentItem, Constants.PpSerializeDefault);
    }

    public static void SaveCurrentItem(string itemName)
    {
        Mock.SetString(Constants.PpCurrentItem, itemName);
    }

    public static string GetCurrentItemName()
    {
        var itemName = Mock.GetString(Constants.PpCurrentItem, Constants.PpSerializeDefault);
        return itemName;
    }

    public static Item GetCurrentItem()
    {
        var itemName = Mock.GetString(Constants.PpCurrentItem, Constants.PpSerializeDefault);
        if (string.IsNullOrEmpty(itemName))
            return null;
        else
        {
            var currentItem = (Item)Activator.CreateInstance(Type.GetType("Item" + itemName.Replace(" ", "").Replace("'", "").Replace("-", "")));
            return currentItem;
        }
    }

    public static void ResetTattoos()
    {
        Mock.SetString(Constants.PpCurrentTattoos, null);
        Mock.SetString(Constants.PpCurrentBodyParts, null);
        Mock.SetString(Constants.PpCurrentMaxedOutTattoos, null);
    }

    public static BodyPart AddTattoo(string name)
    {
        var tattoosFullStr = Mock.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
        if (tattoosFullStr == null)
            tattoosFullStr = "";
        var nameToAdd = name.Replace(" ", "").Replace("'", "").Replace("-", "");
        var alreadyStart = tattoosFullStr.IndexOf(nameToAdd);
        var alreadyBodyPartsIds = Mock.GetString(Constants.PpCurrentBodyParts);
        if (alreadyBodyPartsIds == null)
            alreadyBodyPartsIds = "";
        var newBodyPartStr = "";
        var tattooModel = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + nameToAdd));
        if (alreadyStart != -1)
        {
            var tattooFullStrSplits = tattoosFullStr.Split(';');
            for (int i = 0; i < tattooFullStrSplits.Length; ++i)
            {
                if (!tattooFullStrSplits[i].Contains(nameToAdd))
                    continue;
                var tattooStr = tattooFullStrSplits[i];
                var separatorLevelId = tattooStr.LastIndexOf('L');
                int currentTattooLevel = int.Parse(tattooStr.Substring(0 + nameToAdd.Length + 1, 2));
                if (currentTattooLevel + 1 > tattooModel.MaxLevel)
                    return BodyPart.MaxLevelReached;
                tattoosFullStr = tattoosFullStr.Replace(nameToAdd + "L" + currentTattooLevel.ToString("00"), nameToAdd + "L" + (++currentTattooLevel).ToString("00"));
                newBodyPartStr = "-10"; // SO FUCKING UGLY
                if (currentTattooLevel == tattooModel.MaxLevel)
                    AddMaxedOutTattoo(tattooModel.Id);
            }
        }
        else if (alreadyBodyPartsIds.Length < Constants.AvailableBodyPartsIds.Length)
        {
            var availablesPartsIds = GetRemainingAvailablesPartsIds(alreadyBodyPartsIds);
            var newBodyPartId = UnityEngine.Random.Range(0, availablesPartsIds.Length / 2);
            newBodyPartStr = availablesPartsIds.Substring(newBodyPartId * 2, 2);
            alreadyBodyPartsIds += newBodyPartStr;
            tattoosFullStr += nameToAdd + "L01B" + newBodyPartStr + ";";
            if (tattooModel.MaxLevel == 1)
                AddMaxedOutTattoo(tattooModel.Id);
            //Debug.Log($"alreadyBodyPartsIds: {alreadyBodyPartsIds}");
            Mock.SetString(Constants.PpCurrentBodyParts, alreadyBodyPartsIds);
        }

        Mock.SetString(Constants.PpCurrentTattoos, tattoosFullStr);
        return string.IsNullOrEmpty(newBodyPartStr) ? BodyPart.None : (BodyPart)int.Parse(newBodyPartStr);
    }

    public static string GetRemainingAvailablesPartsIds(string alreadyBodyPartsIds = null)
    {
        if (alreadyBodyPartsIds == null)
            alreadyBodyPartsIds = Mock.GetString(Constants.PpCurrentBodyParts);
        if (alreadyBodyPartsIds == null)
            alreadyBodyPartsIds = "";
        var availablesPartsIds = Constants.AvailableBodyPartsIds;
        for (int i = 0; i < alreadyBodyPartsIds.Length; i += 2)
        {
            int id = int.Parse(alreadyBodyPartsIds.Substring(i, 2));
            var matchingId = -1;
            for (int j = 0; j < availablesPartsIds.Length; j += 2)
            {
                if (availablesPartsIds[j] == id.ToString("00")[0] && availablesPartsIds[j + 1] == id.ToString("00")[1])
                {
                    matchingId = j;
                    break;
                }
            }
            if (matchingId != -1 && matchingId % 2 == 0)
                availablesPartsIds = availablesPartsIds.Remove(matchingId, 2);
        }
        return availablesPartsIds;
    }

    public static string GetCurrentTattoosString()
    {
        var tattoos = Mock.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
        return tattoos;
    }

    public static List<Tattoo> GetCurrentTattoos()
    {
        var tattoosFullStr = Mock.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
        var tattoosList = new List<Tattoo>();
        if (tattoosFullStr == Constants.PpSerializeDefault)
            return tattoosList;
        var tattooFullStrSplits = tattoosFullStr.Split(';');
        for (int i = 0; i < tattooFullStrSplits.Length; ++i)
        {
            var tattooStr = tattooFullStrSplits[i];
            var separatorLevelId = tattooStr.LastIndexOf('L');
            if (separatorLevelId == -1)
                break;
            var tmpTattoo = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + tattooStr.Substring(0, separatorLevelId)));
            tmpTattoo.Level = int.Parse(tattooStr.Substring(separatorLevelId + 1, 2));
            tmpTattoo.BodyPart = (BodyPart)int.Parse(tattooStr.Substring(separatorLevelId + 4, 2));
            tattoosList.Add(tmpTattoo);
        }
        return tattoosList;
    }

    public static Tattoo GetCurrentInkedTattoo(string name)
    {
        var tattoosFullStr = Mock.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
        if (tattoosFullStr == Constants.PpSerializeDefault)
            return null;
        var parsedName = name.Replace(" ", "").Replace("'", "").Replace("-", "");
        var tattooFullStrSplits = tattoosFullStr.Split(';');
        for (int i = 0; i < tattooFullStrSplits.Length; ++i)
        {
            if (!tattooFullStrSplits[i].Contains(parsedName))
                continue;
            var tattooStr = tattooFullStrSplits[i];
            var separatorLevelId = tattooStr.LastIndexOf('L');
            var tmpTattoo = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + tattooStr.Substring(0, separatorLevelId)));
            tmpTattoo.Level = int.Parse(tattooStr.Substring(separatorLevelId + 1, 2));
            tmpTattoo.BodyPart = (BodyPart)int.Parse(tattooStr.Substring(separatorLevelId + 4, 2));
            return tmpTattoo;
        }
        return null;
    }

    public static void AddMaxedOutTattoo(int id)
    {
        var alreadyString = Mock.GetString(Constants.PpCurrentMaxedOutTattoos, Constants.PpSerializeDefault);
        if (alreadyString == null)
            alreadyString = "";
        alreadyString += $"{id};";
        Mock.SetString(Constants.PpCurrentMaxedOutTattoos, alreadyString);
    }

    public static List<int> GetMaxedOutTattoos()
    {
        var alreadyMaxedString = Mock.GetString(Constants.PpCurrentMaxedOutTattoos, Constants.PpRunAlreadyDialogDefault);
        var alreadyMaxedTattoos = new List<int>();
        if (!string.IsNullOrEmpty(alreadyMaxedString))
        {
            var test = alreadyMaxedString.Split(';');
            for (int i = 0; i < test.Length; ++i)
            {
                if (!string.IsNullOrEmpty(test[i]))
                    alreadyMaxedTattoos.Add(int.Parse(test[i]));
            }
        }
        return alreadyMaxedTattoos;
    }

    public static void SaveEffectsLevel(float level)
    {
        PlayerPrefs.SetFloat(Constants.PpEffectsLevel, level);
    }

    public static float GetEffectsLevel()
    {
        var level = PlayerPrefs.GetFloat(Constants.PpEffectsLevel, Constants.PpAudioLevelDefault);
        return level;
    }

    public static void SaveVibrationEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(Constants.PpVibrationEnabled, enabled ? 1 : 0);
    }

    public static bool GetVibrationEnabled()
    {
        var enabled = PlayerPrefs.GetInt(Constants.PpVibrationEnabled, Constants.PpVibrationEnabledDefault == true ? 1 : 0);
        return enabled == 1 ? true : false;
    }

    public static void SaveMusicLevel(float level)
    {
        PlayerPrefs.SetFloat(Constants.PpMusicLevel, level);
    }

    public static float GetMusicLevel()
    {
        var level = PlayerPrefs.GetFloat(Constants.PpMusicLevel, Constants.PpAudioLevelDefault);
        return level;
    }

    public static void SaveTouchSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat(Constants.PpTouchSensitivity, sensitivity);
    }

    public static float GetTouchSensitivity()
    {
        var sensitivity = PlayerPrefs.GetFloat(Constants.PpTouchSensitivity, Constants.PpTouchSensitivityDefault);
        return sensitivity;
    }

    public static void SaveScanlinesHardness(int hardness)
    {
        PlayerPrefs.SetInt(Constants.PpScanlinesHardness, hardness);
    }

    public static int GetScanlinesHardness()
    {
        var hardness = PlayerPrefs.GetInt(Constants.PpScanlinesHardness, Constants.PpScanlinesHardnessDefault);
        return hardness;
    }

    public static void SaveFullscreen(bool enabled)
    {
        PlayerPrefs.SetInt(Constants.PpFullScreen, enabled ? 1 : 0);
    }

    public static bool GetFullscreen()
    {
        var enabled = PlayerPrefs.GetInt(Constants.PpFullScreen, Constants.PpFullScreenDefault == true ? 1 : 0);
        return enabled == 1 ? true : false;
    }

    public static void SaveResolution(int resolutionId)
    {
        PlayerPrefs.SetInt(Constants.PpResolution, resolutionId);
    }

    public static int GetResolution()
    {
        var resolutionId = PlayerPrefs.GetInt(Constants.PpResolution, Constants.PpResolutionDefault);
        return resolutionId;
    }

    public static void SaveLastMaxResolution(int height)
    {
        PlayerPrefs.SetInt(Constants.PpLastMaxResolution, height);
    }

    public static int GetLastMaxResolution()
    {
        var height = PlayerPrefs.GetInt(Constants.PpLastMaxResolution, Constants.PpLastMaxResolutionDefault);
        return height;
    }

    public static void SaveIsInFight(bool isInFight)
    {
        Mock.SetInt(Constants.PpIsInFight, isInFight == true ? 1 : 0);
    }

    public static bool GetIsInFight()
    {
        var IsInFight = Mock.GetInt(Constants.PpIsInFight, Constants.PpSerializeDefaultInt);
        return IsInFight == 1 ? true : false;
    }

    public static void SaveDas(int das)
    {
        PlayerPrefs.SetInt(Constants.PpDas, das);
    }

    public static int GetDas()
    {
        if (Constants.CurrentGameMode == GameMode.TrainingOldSchool)
            return Constants.OldSchoolDas;
        var das = PlayerPrefs.GetInt(Constants.PpDas, Constants.PpDasDefault);
        return das;
    }

    public static void SaveArr(int arr)
    {
        PlayerPrefs.SetInt(Constants.PpArr, arr);
    }

    public static int GetArr()
    {
        if (Constants.CurrentGameMode == GameMode.TrainingOldSchool)
            return Constants.OldSchoolArr;
        var arr = PlayerPrefs.GetInt(Constants.PpArr, Constants.PpArrDefault);
        return arr;
    }

    public static void SaveRotationPoint(bool enable)
    {
        PlayerPrefs.SetInt(Constants.PpRotationPoint, enable ? 1 : 0);
    }

    public static bool GetRotationPoint()
    {
        var rotationPoint = PlayerPrefs.GetInt(Constants.PpRotationPoint, Constants.PpRotationPointDefault == true ? 1 : 0);
        return rotationPoint == 1 ? true : false;
    }

    public static void SaveClassicPieces(bool enable)
    {
        PlayerPrefs.SetInt(Constants.PpClassicPieces, enable ? 1 : 0);
    }

    public static bool GetClassicPieces()
    {
        var classicPieces = PlayerPrefs.GetInt(Constants.PpClassicPieces, Constants.PpClassicPiecesDefault == true ? 1 : 0);
        return classicPieces == 1 ? true : false;
    }

    public static void SaveOrientation(Direction orientation)
    {
        PlayerPrefs.SetInt(Constants.PpOrientation, orientation == Direction.Vertical ? 0 : 1);
    }

    public static Direction GetOrientation()
    {
#if UNITY_ANDROID
        var orientation = PlayerPrefs.GetInt(Constants.PpOrientation, Constants.PpOrientationDefault == Direction.Vertical ? 0 : 1);
        return orientation == 0 ? Direction.Vertical : Direction.Horizontal;
#else
        return Direction.Vertical;
#endif
    }

    public static int GetDialogProgress(string dialogLibelle)
    {
        var progressId = PlayerPrefs.GetInt(dialogLibelle, Constants.PpSerializeDefaultInt);
        return progressId;
    }

    public static void SaveDialogProgress(string dialogLibelle, int id)
    {
        PlayerPrefs.SetInt(dialogLibelle, id);
    }

    public static List<string> GetAlreadyDialog()
    {
        var alreadyString = PlayerPrefs.GetString(Constants.PpRunAlreadyDialog, Constants.PpRunAlreadyDialogDefault);
        var alreadyDialogs = new List<string>();
        if (alreadyString != null)
        {
            var test = alreadyString.Split('@');
            for (int i = 0; i < test.Length; ++i)
                alreadyDialogs.Add(test[i]);
        }
        return alreadyDialogs;
    }

    public static void AddToAlreadyDialog(string dialog)
    {
        var alreadyString = PlayerPrefs.GetString(Constants.PpRunAlreadyDialog, Constants.PpRunAlreadyDialogDefault);
        alreadyString += $"{dialog}@";
        PlayerPrefs.SetString(Constants.PpRunAlreadyDialog, alreadyString);
    }

    public static void ResetAlreadyDialog()
    {
        PlayerPrefs.SetString(Constants.PpRunAlreadyDialog, Constants.PpRunAlreadyDialogDefault);
    }

    public static int GetBonusRarePercent()
    {
        var bonus = Mock.GetInt(Constants.PpBonusRarePercent, Constants.PpSerializeDefaultInt);
        return bonus;
    }

    public static void SaveBonusRarePercent(int amount)
    {
        Mock.SetInt(Constants.PpBonusRarePercent, amount);
    }

    public static void IncrementBonusRarePercent(int times)
    {
        var bonus = Mock.GetInt(Constants.PpBonusRarePercent, Constants.PpSerializeDefaultInt);
        Mock.SetInt(Constants.PpBonusRarePercent, bonus + times);
    }

    public static int GetBonusLegendaryPercent()
    {
        var bonus = Mock.GetInt(Constants.PpBonusLegendaryPercent, Constants.PpSerializeDefaultInt);
        return bonus;
    }

    public static void SaveBonusLegendaryPercent(int amount)
    {
        Mock.SetInt(Constants.PpBonusLegendaryPercent, amount);
    }

    public static void IncrementBonusLegendaryPercent(int times)
    {
        var bonus = Mock.GetInt(Constants.PpBonusLegendaryPercent, Constants.PpSerializeDefaultInt);
        Mock.SetInt(Constants.PpBonusLegendaryPercent, bonus + times);
    }

    public static int GetRunBossVanquished()
    {
        var run = Mock.GetInt(Constants.PpRunBossVanquished, Constants.PpSerializeDefaultInt);
        return run;
    }

    public static void IncrementRunBossVanquished()
    {
        var run = Mock.GetInt(Constants.PpRunBossVanquished, Constants.PpSerializeDefaultInt);
        Mock.SetInt(Constants.PpRunBossVanquished, run + 1);
    }

    public static void ResetRunBossVanquished()
    {
        Mock.SetInt(Constants.PpRunBossVanquished, Constants.PpSerializeDefaultInt);
    }

    public static int GetRealmBossProgression()
    {
        var realmBossProgression = Mock.GetInt(Constants.PpRealmBossProgression, Constants.PpRealmBossProgressionDefault);
        return realmBossProgression;
    }

    public static void SaveRealmBossProgression(int realmId)
    {
        Mock.SetInt(Constants.PpRealmBossProgression, realmId);
    }

    public static List<int> GetTotalResources()
    {
        var resourcesStr = Mock.GetString(Constants.PpTotalResources, Constants.PpTotalResourcesDefault);
        var resources = new List<int>();
        if (!string.IsNullOrEmpty(resourcesStr))
        {
            var resourcesStrSplit = resourcesStr.Split(';');
            for (int i = 0; i < ResourcesData.Resources.Length; ++i)
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

    public static void AlterResource(int resourceId, int amount)
    {
        var resources = GetTotalResources();
        resources[resourceId] += amount;
        var resourcesStr = "";
        foreach (int resource in resources)
        {
            resourcesStr += $"{resource};";
        }
        Mock.SetString(Constants.PpTotalResources, resourcesStr);
    }

    public static string GetBoughtTreeNodes()
    {
        var boughtTreeNodes = Mock.GetString(Constants.PpBoughtTreeNodes, Constants.PpSerializeDefault);
        if (string.IsNullOrEmpty(boughtTreeNodes))
            boughtTreeNodes = "";
        return boughtTreeNodes;
    }

    public static void ResetBoughtTreeNodes()
    {
        Mock.SetString(Constants.PpBoughtTreeNodes, Constants.PpSerializeDefault);
        ResetRealmTree();
    }

    public static void AddBoughtTreeNode(string nodeName, NodeType nodeType)
    {
        var alreadyBoughtTreeNodesStr = GetBoughtTreeNodes();
        Mock.SetString(Constants.PpBoughtTreeNodes, $"{alreadyBoughtTreeNodesStr}{nodeName} ");

        var realmTree = GetRealmTree();
        switch (nodeType)
        {
            case NodeType.AttackBoost: realmTree.AttackBoost += 1; break;
            case NodeType.CooldownBrake: realmTree.CooldownBrake += 1; break;
            case NodeType.CriticalPrecision: realmTree.CriticalPrecision += 2; break;
            case NodeType.PosthumousItem: realmTree.PosthumousItem += 1; break;
            case NodeType.LockDelay: realmTree.LockDelay += 0.25f; break;
            case NodeType.LifeRoulette: realmTree.LifeRoulette += 50; break;
            case NodeType.BossHate: realmTree.BossHate += 10; break;
            case NodeType.Shadowing: realmTree.Shadowing += 1; break;
            case NodeType.Repentance: realmTree.Repentance += 1; break;
        }
        SaveRealmTree(realmTree);
    }

    public static void SaveBoughtTreeNodes(string boughtTreeNodes)
    {
        Mock.SetString(Constants.PpBoughtTreeNodes, boughtTreeNodes);
    }

    public static void SaveRealmTree(RealmTree realmTree)
    {
        Mock.SetString(Constants.PpRealmTree, JsonUtility.ToJson(realmTree));
    }

    public static RealmTree GetRealmTree()
    {
        var realmTree = JsonUtility.FromJson<RealmTree>(Mock.GetString(Constants.PpRealmTree, Constants.PpSerializeDefault));
        if (realmTree == null)
            return new RealmTree();
        return realmTree;
    }

    public static void ResetRealmTree()
    {
        SaveRealmTree(null);
    }

    public static void SaveVersion(string version)
    {
        Mock.SetString(Constants.PpVersion, version);
    }

    public static string GetVersion()
    {
        var version = Mock.GetString(Constants.PpVersion, Constants.PpSerializeDefault);
        return version;
    }

    public static void SaveInfernalUnlocked(bool unlocked)
    {
        Mock.SetInt(Constants.PpInfernalUnlocked, unlocked ? 1 : 0);
    }

    public static void SaveDivineUnlocked(bool unlocked)
    {
        Mock.SetInt(Constants.PpDivineUnlocked, unlocked ? 1 : 0);
    }

    public static bool GetInfernalUnlocked()
    {
        var randomItemMaxRarity = GetRealmBossProgression();
        var unlocked = Mock.GetInt(Constants.PpInfernalUnlocked, 0);
        return unlocked == 1 || randomItemMaxRarity >= 0 ? true : false;
    }

    public static bool GetDivineUnlocked()
    {
        var unlocked = Mock.GetInt(Constants.PpDivineUnlocked, 0);
        return unlocked == 1 ? true : false;
    }

    public static AccountDto GetLastSavedCredentials()
    {
        var credentials = PlayerPrefs.GetString(Constants.PpLastSavedCredentials, Constants.PpSerializeDefault);
        if (credentials == null)
            return null;
        var splits = credentials.Split('|');
        if (splits == null || splits.Length < 2)
            return null;
        return new AccountDto(splits[0], splits[1], "", "", -1, "");
    }

    public static void SaveLastSavedCredentials(AccountDto account)
    {
        if (account == null)
        {
            PlayerPrefs.SetString(Constants.PpLastSavedCredentials, null);
            return;
        }
        var credentials = $"{account.PlayerName}|{account.Password}";
        PlayerPrefs.SetString(Constants.PpLastSavedCredentials, credentials);
    }

    public static void IncrementNumberRunWithoutCharacterEncounter()
    {
        var nbRuns = GetNumberRunWithoutCharacterEncounter();
        Mock.SetInt(Constants.PpNumberRunsWithoutCharacterEncounter, nbRuns + 1);
    }

    public static void ResetNumberRunWithoutCharacterEncounter(int resetValue)
    {
        Mock.SetInt(Constants.PpNumberRunsWithoutCharacterEncounter, resetValue);
    }

    public static int GetNumberRunWithoutCharacterEncounter()
    {
        var nbRuns = Mock.GetInt(Constants.PpNumberRunsWithoutCharacterEncounter, Constants.PpSerializeDefaultInt);
        return nbRuns;
    }

    public static void SaveHasMetBeholder(bool hasMetHim)
    {
        PlayerPrefs.SetInt(Constants.PpHasMetBeholder, hasMetHim ? 1 : 0);
    }

    public static bool GetHasMetBeholder()
    {
        var hasMetHim = PlayerPrefs.GetInt(Constants.PpHasMetBeholder, Constants.PpSerializeDefaultInt);
        return hasMetHim == 1 ? true : false;
    }
}
