using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SplashScreenBhv : SceneBhv
{
    private TMPro.TextMeshPro _catchPhrase;
    private List<string> _catchPhrases;

    void Start()
    {
        // [DEBUG]
        //PlayerPrefs.DeleteAll();
        Init();
    }

    protected override void Init()
    {
        base.Init();
        var resolutionService = GetComponent<ResolutionService>();
        PlayerPrefsHelper.SaveResolution(resolutionService.SetResolution(PlayerPrefsHelper.GetResolution()));
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
            "How are you doing ?",
            "My regards",
            "Back in the game",
            "Since 2020",
            "Do you like the game ?",
            "Looking goood",
            "Hey you"
        };
        if (DateTime.Today.DayOfWeek == DayOfWeek.Monday || DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
            _catchPhrases.Add("Urgh... [DAY]s am I right ?");
        else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
            _catchPhrases.Add("It's [DAY] my dudes");
        _catchPhrase = GameObject.Find("CatchPhrase").GetComponent<TMPro.TextMeshPro>();
        var tmpTxt = _catchPhrases[UnityEngine.Random.Range(0, _catchPhrases.Count)];
        tmpTxt = tmpTxt.Replace("[DAY]", DateTime.Today.DayOfWeek.ToString());
        _catchPhrase.text = tmpTxt;
        GameObject.Find("Title").GetComponent<ButtonBhv>().EndActionDelegate = GoToMainMenu;
    }

    private void GoToMainMenu()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            Constants.CurrentMusicType = MusicType.Menu;
            NavigationService.NewRootScene(Constants.MainMenuScene);
            return true;
        }
    }
}
