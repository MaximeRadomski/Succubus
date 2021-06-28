using System;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenBhv : SceneBhv
{
    private TMPro.TextMeshPro _catchPhrase;
    private List<string> _catchPhrases;

    public override MusicType MusicType => MusicType.SplashScreen;

    void Start()
    {
        Init();

        //var highScores = new List<HighScoreDto>()
        //{
        //    new HighScoreDto("Abject", 1000000, 0),
        //    new HighScoreDto("Coco", 15000, 1),
        //    new HighScoreDto("Waga", 135498, 2),
        //    new HighScoreDto("Taüff", 8465, 3),
        //    new HighScoreDto("Segah", 98646, 4),
        //    new HighScoreDto("Oldo", 8764, 5),
        //    new HighScoreDto("Alka", 666666666, 6)
        //};
        //Instantiator.NewLoading();
        //DatabaseService.PutHighScores(highScores, () =>
        //{
        //    DatabaseService.GetHighScores((List<HighScoreDto> results) =>
        //    {
        //        _catchPhrase.text = "";
        //        foreach (var result in results)
        //        {
        //            _catchPhrase.text += $"{result.PlayerNameId}: {result.Score}\n";
        //            Helper.ResumeLoading();
        //        }
        //    });
        //});
    }

    protected override void Init()
    {
        base.Init();
#if !UNITY_ANDROID
        var resolutionService = GetComponent<ResolutionService>();
        PlayerPrefsHelper.SaveResolution(resolutionService.SetResolution(PlayerPrefsHelper.GetResolution()));
#endif
        var canCheckOnlineVersionRightAway = CheckLastInstalledVersion();
        if (canCheckOnlineVersionRightAway)
            CheckLastUpdatedVersion();
        _catchPhrases = new List<string>()
        {
            "Welcome, sinner",
            "Glad to see you back",
            "Here we go again",
            "Always a pleasure",
            "Welcome back",
            "Oh hi there",
            "Good day to you",
            "Nice to see you here",
            "Excellent shape my dear",
            "Ready to burst some angel ass",
            "How are you doing?",
            "My regards",
            "Back in the game",
            "Since 2020",
            "Do you like the game?",
            "Looking goood",
            "Hey you"
        };
        if (DateTime.Today.DayOfWeek == DayOfWeek.Monday || DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
            _catchPhrases.Add("Urgh... [DAY]s am I right?");
        else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
            _catchPhrases.Add("It's [DAY] my dudes!");
        _catchPhrase = GameObject.Find("CatchPhrase").GetComponent<TMPro.TextMeshPro>();
        var tmpTxt = _catchPhrases[UnityEngine.Random.Range(0, _catchPhrases.Count)];
        tmpTxt = tmpTxt.Replace("[DAY]", DateTime.Today.DayOfWeek.ToString());
        _catchPhrase.text = tmpTxt;
        GameObject.Find("Title").GetComponent<ButtonBhv>().EndActionDelegate = GoToMainMenu;
    }

    private bool CheckLastInstalledVersion()
    {
        var lastSavedVersionStr = PlayerPrefsHelper.GetVersion();
        string[] lastSavedVersion = null;
        if (!string.IsNullOrEmpty(lastSavedVersionStr))
            lastSavedVersion = lastSavedVersionStr.Split('.');
        var currentVersion = Application.version.Split('.');

        if (lastSavedVersion == null || !int.TryParse(lastSavedVersion[0], out var main))
        {
            PlayerPrefs.DeleteAll();
        }
        else if (int.Parse(lastSavedVersion[0]) < int.Parse(currentVersion[0]))
        {
            //PlayerPrefs.DeleteAll();
            //Instantiator.NewPopupYesNo("Old version", "your last installed version was outdated. your saved preferences and progression have been restored to their default value.", null, "Ok", null);
        }
#if !UNITY_ANDROID
        //Check under 01.03.001: Secured PlayerPrefs. Reset all !!!
        else if (IsUnder(lastSavedVersion, new string[] { "1", "03", "001" }))
        {
            PlayerPrefs.DeleteAll();
            Instantiator.NewPopupYesNo("New Data Management", "your previous installation had outdated data management. your data have been reset to its default values.", null, "Ok", (result) =>
            {
                CheckLastUpdatedVersion();
                return true;
            });
            return false;
        }
#endif

        PlayerPrefsHelper.SaveVersion(Application.version);
        return true;
    }

    private void CheckLastUpdatedVersion()
    {
        DatabaseService.GetLastUpdatedVersion((lastUpdatedVersionStr) =>
        {
            if (string.IsNullOrEmpty(lastUpdatedVersionStr))
                return;
            string[] lastUpdatedVersion = null;
            lastUpdatedVersion = lastUpdatedVersionStr.Split('.');
            var currentVersion = Application.version.Split('.');
            if (IsUnder(currentVersion, lastUpdatedVersion))
            {
                Instantiator.NewPopupYesNo("Update", "a new update is available!\nredirecting to download page?", "Not now", "Sure", (goOnline) =>
                {
                    if (!goOnline)
                        return false;
                    Application.OpenURL("https://abject.itch.io/infidhells/");
                    return true;
                });
            }
        });
    }

    private bool IsUnder(string[] version1, string[] version2)
    {
        var intVersion = (int.Parse(version1[0]) * 100000) + (int.Parse(version1[1]) * 1000) + (int.Parse(version1[2]));
        var intToCheck = (int.Parse(version2[0]) * 100000) + (int.Parse(version2[1]) * 1000) + (int.Parse(version2[2]));
        return intVersion < intToCheck;
    }

    private void GoToMainMenu()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.NewRootScene(Constants.MainMenuScene);
            return true;
        }
    }
}
