using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionService : MonoBehaviour
{
    private static readonly string TableProgressions = "Progressions";

    public static void PutProgression(ProgressionDto progression, string playerName, Action onResolved)
    {
        RestClient.Put<ProgressionDto>(DatabaseService.SetTableAndId(TableProgressions, playerName), progression).Then(r =>
        {
            onResolved.Invoke();
        });
    }

    public static void GetProgression(string playerNameId, Action<ProgressionDto> resultAction)
    {
        RestClient.Get(DatabaseService.SetTableAndId(TableProgressions, playerNameId)).Then(returnValue =>
        {
            ProgressionDto progression = null;
            if (!string.IsNullOrEmpty(returnValue.Text) && returnValue.Text != "null")
                progression = JsonUtility.FromJson<ProgressionDto>(returnValue.Text);
            resultAction.Invoke(progression);
        });
    }

    public static void ApplyProgression(ProgressionDto onlineProgression)
    {
        PlayerPrefsHelper.SaveUnlockedCharacters(onlineProgression.UnlockedCharacters);
        PlayerPrefsHelper.SaveUnlockedSkins(onlineProgression.UnlockedSkins);
        var realmTree = JsonUtility.FromJson<RealmTree>(onlineProgression.RealmTree);
        if (realmTree == null)
            realmTree = new RealmTree();
        PlayerPrefsHelper.SaveRealmTree(realmTree);
        PlayerPrefsHelper.SaveBoughtTreeNodes(onlineProgression.BoughtTreeNodes);
        PlayerPrefsHelper.SaveBonusRarePercent(onlineProgression.BonusRarePercent);
        PlayerPrefsHelper.SaveBonusLegendaryPercent(onlineProgression.BonusLegendaryPercent);
        PlayerPrefsHelper.SaveRealmBossProgression(onlineProgression.RealmBossProgression);
    }

    public static void DeleteProgression()
    {
        PlayerPrefsHelper.SaveUnlockedCharacters(Constants.PpUnlockedCharactersDefault);
        PlayerPrefsHelper.SaveUnlockedSkins(Constants.PpUnlockedSkinsDefault);
        PlayerPrefsHelper.ResetBoughtTreeNodes();
        PlayerPrefsHelper.SaveBonusRarePercent(Constants.PpSerializeDefaultInt);
        PlayerPrefsHelper.SaveBonusLegendaryPercent(Constants.PpSerializeDefaultInt);
        PlayerPrefsHelper.SaveRealmBossProgression(Constants.PpRealmBossProgressionDefault);
        PlayerPrefsHelper.ResetCinematicsWatched();
        PlayerPrefsHelper.ResetRun();
        PlayerPrefsHelper.ResetRunBossVanquished();
        PlayerPrefsHelper.SaveHasMetBeholder(false);
        PlayerPrefsHelper.SaveHasMetLurker(false);
        PlayerPrefsHelper.ResetDifficulties();
        PlayerPrefsHelper.SaveIsInFight(false);
        PlayerPrefsHelper.ResetTotalResources();   
        foreach (var dialogKey in DialogsData.DialogTree.Keys)
        {
            if (PlayerPrefs.GetInt(dialogKey, -1) != -1)
                PlayerPrefsHelper.SaveDialogProgress(dialogKey, Constants.PpSerializeDefaultInt);
        }
        PlayerPrefsHelper.SaveInfernalUnlocked(false);
        PlayerPrefsHelper.SaveDivineUnlocked(false);
    }
}
