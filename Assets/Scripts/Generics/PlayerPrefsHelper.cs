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
        PlayerPrefs.SetString(Constants.PpRun, JsonUtility.ToJson(run));
    }

    public static Run GetRun()
    {
        var run = JsonUtility.FromJson<Run>(PlayerPrefs.GetString(Constants.PpRun, Constants.PpSerializeDefault));
        if (run == null)
            return null;
        return run;
    }

    public static void ResetRun()
    {
        SaveRun(null);
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

    public static void SaveTraining(int score, int level, int lines, int pieces)
    {
        PlayerPrefs.SetInt(Constants.PpTrainingScore, score);
        PlayerPrefs.SetInt(Constants.PpTrainingLevel, level);
        PlayerPrefs.SetInt(Constants.PpTrainingLines, lines);
        PlayerPrefs.SetInt(Constants.PpTrainingPieces, pieces);
    }

    public static void ResetTraining()
    {
        PlayerPrefs.SetInt(Constants.PpTrainingScore, 0);
        PlayerPrefs.SetInt(Constants.PpTrainingLevel, 1);
        PlayerPrefs.SetInt(Constants.PpTrainingLines, 0);
        PlayerPrefs.SetInt(Constants.PpTrainingPieces, 0);
    }

    public static List<int> GetTraining()
    {
        var results = new List<int>();
        results.Add(PlayerPrefs.GetInt(Constants.PpTrainingScore, 0));
        results.Add(PlayerPrefs.GetInt(Constants.PpTrainingLevel, 1));
        results.Add(PlayerPrefs.GetInt(Constants.PpTrainingLines, 0));
        results.Add(PlayerPrefs.GetInt(Constants.PpTrainingPieces, 0));
        return results;
    }

    public static void SaveTrainingHightScore(int score)
    {
        PlayerPrefs.SetInt(Constants.PpTrainingHighScore, score);
    }

    public static int GetTrainingHighScore()
    {
        var highScore = PlayerPrefs.GetInt(Constants.PpTrainingHighScore, 0);
        return highScore;
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

    public static void SaveOrientation(string orientation)
    {
        PlayerPrefs.SetString(Constants.PpOrientation, orientation);
    }

    public static string GetOrientation()
    {
        var orientation = PlayerPrefs.GetString(Constants.PpOrientation, Constants.PpOrientationDefault);
        return orientation;
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

    public static void AddUnlockedCharacters(Character character)
    {
        var currentUnlockedCharactersString = GetUnlockedCharactersString();
        currentUnlockedCharactersString = currentUnlockedCharactersString.ReplaceChar(character.Id, '1');
        SaveUnlockedCharacters(currentUnlockedCharactersString);
    }

    public static void SaveUnlockedCharacters(string unlockedCharacters)
    {
        PlayerPrefs.SetString(Constants.PpUnlockedCharacters, unlockedCharacters);
    }

    public static string GetUnlockedCharactersString()
    {
        var unlockedCharacters = PlayerPrefs.GetString(Constants.PpUnlockedCharacters, Constants.PpUnlockedCharactersDefault);
        return unlockedCharacters;
    }

    public static void SaveSelectedCharacter(int selectedCharacter)
    {
        PlayerPrefs.SetInt(Constants.PpSelectedCharacter, selectedCharacter);
    }

    public static int GetSelectedCharacterId()
    {
        var selectedCharacter = PlayerPrefs.GetInt(Constants.PpSelectedCharacter, Constants.PpSelectedCharacterDefault);
        return selectedCharacter;
    }

    public static void SaveRunCharacter(Character character)
    {
        PlayerPrefs.SetString(Constants.PpRunCharacter, JsonUtility.ToJson(character));
    }

    public static Character GetRunCharacter()
    {
        var character = JsonUtility.FromJson<Character>(PlayerPrefs.GetString(Constants.PpRunCharacter, Constants.PpSerializeDefault));
        if (character == null)
            return CharactersData.Characters[GetSelectedCharacterId()];
        return character;
    }

    public static void SaveCurrentOpponents(List<Opponent> opponents)
    {
        if (opponents == null)
        {
            PlayerPrefs.SetString(Constants.PpCurrentOpponents, Constants.PpSerializeDefault);
            return;
        }
        var opponentsStr = "";
        foreach (var opponent in opponents)
        {
            opponentsStr += opponent.Realm.GetHashCode() + ":" + opponent.Id + ";";
        }
        PlayerPrefs.SetString(Constants.PpCurrentOpponents, opponentsStr);
    }

    public static List<Opponent> GetCurrentOpponents(Run run)
    {
        var opponentsStr = PlayerPrefs.GetString(Constants.PpCurrentOpponents, Constants.PpSerializeDefault);
        int i = 0;
        var opponentsList = new List<Opponent>();
        while (!string.IsNullOrEmpty(opponentsStr) || i > 15)
        {
            var separatorRealmId = opponentsStr.IndexOf(':');
            var separatorId = opponentsStr.IndexOf(';');
            if (separatorId == -1)
                break;
            //var realmId = int.Parse(opponentsStr.Substring(0, separatorRealmId));
            var tmpId = int.Parse(opponentsStr.Substring(separatorRealmId + 1, separatorId - (separatorRealmId + 1)));
            Opponent tmpOpponent = null;
            if (run.CurrentRealm == Realm.Hell)
                tmpOpponent = OpponentsData.HellOpponents[tmpId];
            else if (run.CurrentRealm == Realm.Earth)
                tmpOpponent = OpponentsData.EarthOpponents[tmpId];
            else if (run.CurrentRealm == Realm.Heaven)
                tmpOpponent = OpponentsData.HeavenOpponents[tmpId];
            opponentsList.Add(tmpOpponent);
            if (separatorId + 1 >= opponentsStr.Length)
                break;
            opponentsStr = opponentsStr.Substring(separatorId + 1);
            ++i;
        }
        return opponentsList;
    }

    public static void ResetCurrentItem()
    {
        PlayerPrefs.SetString(Constants.PpCurrentItem, Constants.PpSerializeDefault);
    }

    public static void SaveCurrentItem(string itemName)
    {
        PlayerPrefs.SetString(Constants.PpCurrentItem, itemName);
    }

    public static string GetCurrentItemName()
    {
        var itemName = PlayerPrefs.GetString(Constants.PpCurrentItem, Constants.PpSerializeDefault);
        return itemName;
    }

    public static Item GetCurrentItem()
    {
        var itemName = PlayerPrefs.GetString(Constants.PpCurrentItem, Constants.PpSerializeDefault);
        if (string.IsNullOrEmpty(itemName))
            return null;
        else
        {
            var currentItem = (Item)Activator.CreateInstance(Type.GetType("Item" + itemName.Replace(" ", "").Replace("'", "")));
            return currentItem;
        }
    }

    public static void ResetTattoos()
    {
        PlayerPrefs.SetString(Constants.PpCurrentTattoos, null);
        PlayerPrefs.SetString(Constants.PpCurrentBodyParts, null);
    }

    public static BodyPart AddTattoo(string name)
    {
        var tattoosStr = PlayerPrefs.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
        if (tattoosStr == null)
            tattoosStr = "";
        var nameToAdd = name.Replace(" ", "").Replace("'", "");
        var alreadyStart = tattoosStr.IndexOf(nameToAdd);
        var alreadyBodyPartsIds = PlayerPrefs.GetString(Constants.PpCurrentBodyParts);
        if (alreadyBodyPartsIds == null)
            alreadyBodyPartsIds = "";
        var newBodyPartStr = "";
        if (alreadyStart != -1)
        {
            var separatorLevelId = tattoosStr.Substring(alreadyStart).IndexOf('L');
            var tattooModel = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + tattoosStr.Substring(alreadyStart, separatorLevelId)));
            int currentTattooLevel = int.Parse(tattoosStr.Substring(alreadyStart + nameToAdd.Length + 1, 2));
            if (currentTattooLevel + 1 > tattooModel.MaxLevel)
                return BodyPart.MaxLevelReached;
            tattoosStr = tattoosStr.Replace(nameToAdd + "L" + currentTattooLevel.ToString("00"), nameToAdd + "L" + (++currentTattooLevel).ToString("00"));
        }
        else if (alreadyBodyPartsIds.Length < Constants.AvailableBodyPartsIds.Length)
        {
            var availablesPartsIds = Constants.AvailableBodyPartsIds;
            for (int i = 0; i < alreadyBodyPartsIds.Length; i += 2)
            {
                int id = int.Parse(alreadyBodyPartsIds.Substring(i, 2));
                if (availablesPartsIds.Contains(id.ToString("00")))
                    availablesPartsIds.Remove(id * 2, 2);
            }
            var newBodyPartId = UnityEngine.Random.Range(0, availablesPartsIds.Length / 2);
            newBodyPartStr = availablesPartsIds.Substring(newBodyPartId * 2, 2);
            alreadyBodyPartsIds += newBodyPartStr;
            tattoosStr += nameToAdd + "L01B" + newBodyPartStr + ";";
            PlayerPrefs.SetString(Constants.PpCurrentBodyParts, alreadyBodyPartsIds);
        }
        
        PlayerPrefs.SetString(Constants.PpCurrentTattoos, tattoosStr);
        return string.IsNullOrEmpty(newBodyPartStr) ? BodyPart.None : (BodyPart)int.Parse(newBodyPartStr);
    }

    public static string GetCurrentTattoosString()
    {
        var tattoos = PlayerPrefs.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
        return tattoos;
    }

    public static List<Tattoo> GetCurrentTattoos()
    {
        var tattoosStr = PlayerPrefs.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
        int i = 0;
        var tattoosList = new List<Tattoo>();
        while (!string.IsNullOrEmpty(tattoosStr) || i > 15)
        {
            var separatorLevelId = tattoosStr.IndexOf('L');
            if (separatorLevelId == -1)
                break;
            var tmpTattoo = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + tattoosStr.Substring(0, separatorLevelId)));
            tmpTattoo.Level = int.Parse(tattoosStr.Substring(separatorLevelId + 1, 2));
            tmpTattoo.BodyPart = (BodyPart)int.Parse(tattoosStr.Substring(separatorLevelId + 4, 2));
            tattoosList.Add(tmpTattoo);
            if (separatorLevelId + 1 >= tattoosStr.Length)
                break;
            tattoosStr = tattoosStr.Substring(tattoosStr.IndexOf(';') + 1);
            ++i;
        }
        return tattoosList;
    }

    public static Tattoo GetCurrentInkedTattoo(string name)
    {
        var tattoosStr = PlayerPrefs.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
        var parsedName = name.Replace(" ", "").Replace("'", "");
        var tattooStartId = tattoosStr.IndexOf(parsedName);
        if (tattooStartId == -1)
            return null;
        tattoosStr = tattoosStr.Substring(tattooStartId);
        var separatorLevelId = tattoosStr.IndexOf('L');
        var tmpTattoo = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + tattoosStr.Substring(0, separatorLevelId)));
        tmpTattoo.Level = int.Parse(tattoosStr.Substring(separatorLevelId + 1, 2));
        tmpTattoo.BodyPart = (BodyPart)int.Parse(tattoosStr.Substring(separatorLevelId + 4, 2));
        return tmpTattoo;
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

    public static void SaveMusicLevel(float level)
    {
        PlayerPrefs.SetFloat(Constants.PpMusicLevel, level);
    }

    public static float GetMusicLevel()
    {
        var level = PlayerPrefs.GetFloat(Constants.PpMusicLevel, Constants.PpAudioLevelDefault);
        return level;
    }
}
