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
    }

    protected override void Init()
    {
        base.Init();
#if !UNITY_ANDROID
        var resolutionService = GetComponent<ResolutionService>();
        PlayerPrefsHelper.SaveResolution(resolutionService.SetResolution(PlayerPrefsHelper.GetResolution()));
#endif
        CheckVersion();
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

    private void CheckVersion()
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
            PlayerPrefs.DeleteAll();
            Instantiator.NewPopupYesNo("Old version", "your last installed version was outdated. your saved preferences and progression have been restored to their default value.", null, "Ok", null);
        }
#if !UNITY_ANDROID
        //Check under 01.02.031: Changed the keybinding mapping after the add of the "restart training" button
        else if (IsUnder(lastSavedVersion, 1, 2, 31))
        {
            PlayerPrefs.DeleteKey(Constants.PpKeyBinding);
            Instantiator.NewPopupYesNo("Outdated input", "your previous installation had outdated input mapping. your controls have been reset to their default values.", null, "Ok", null);
        }
#endif

        PlayerPrefsHelper.SaveVersion(Application.version);
    }

    private bool IsUnder(string[] version, int main, int second, int third)
    {
        return int.Parse(version[0]) <= main && int.Parse(version[1]) <= second && int.Parse(version[2]) < third;
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
