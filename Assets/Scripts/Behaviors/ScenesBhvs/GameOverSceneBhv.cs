using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.GameOver;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        if (Constants.GameOverParams == null)
            Constants.GameOverParams = "Test|Realm|99";
        var gameOverParams = Constants.GameOverParams.Split('|');
        GameObject.Find("OpponentText").GetComponent<TMPro.TextMeshPro>().text = $"by {gameOverParams[0].ToLower()}";
        GameObject.Find("RealmText").GetComponent<TMPro.TextMeshPro>().text = $"{gameOverParams[1].ToLower()}";
        GameObject.Find("LevelText").GetComponent<TMPro.TextMeshPro>().text = $"lvl {gameOverParams[2].ToLower()}";

        var tattoos = PlayerPrefsHelper.GetCurrentTattoos();
        List<int> tattoosSpritesIds = null;
        if (tattoos.Count % 2 == 0)
            tattoosSpritesIds = new List<int>() { 1, 3, 6, 8, 2, 7, 5, 9, 0, 4, 10, 11 };
        else
            tattoosSpritesIds = new List<int>() { 2, 1, 3, 6, 8, 5, 9, 0, 4, 10, 11, 7 };
        int i = 0;
        foreach (Tattoo tattoo in tattoos)
        {
            GameObject.Find($"Tattoo{tattoosSpritesIds[i].ToString("00")}").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + tattoo.Id.ToString("00"));
            ++i;
        }

        GameObject.Find("RollbackButton").GetComponent<ButtonBhv>().EndActionDelegate = GoBackToMainMenu;

        var mult = (float)PlayerPrefsHelper.GetDifficulty();
        if (mult == 0)
            mult = 0.5f;
        int rareAdd = (int)((float)PlayerPrefsHelper.GetRunBossVanquished() * mult);
        int legendaryAdd = rareAdd / 2;
        if (rareAdd >= 1)
        {
            var content = $"{Constants.MaterialHell_3_2}your chance of finding a {Constants.MaterialHell_4_3}rare{Constants.MaterialEnd} loot is increased by {Constants.MaterialHell_4_3}{rareAdd}%{Constants.MaterialEnd}";
            if (legendaryAdd >= 1)
                content = $"{content}\nyour chance of finding a {Constants.MaterialHell_4_3}legendary{Constants.MaterialEnd} loot is increased by {Constants.MaterialHell_4_3}{legendaryAdd}%{Constants.MaterialEnd}";
            Instantiator.NewPopupYesNo("Rarity Boost!", content, null, "Cool!", null);
            PlayerPrefsHelper.IncrementBonusRarePercent(rareAdd);
            PlayerPrefsHelper.IncrementBonusLegendaryPercent(legendaryAdd);
        }
    }

    private void GoBackToMainMenu()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        object OnBlend(bool result)
        {
            NavigationService.LoadBackUntil(Constants.MainMenuScene);
            return true;
        }
    }

    public override void PauseOrPrevious()
    {
        GoBackToMainMenu();
    }
}
