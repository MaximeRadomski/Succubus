using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenBhv : SceneBhv
{
    private TMPro.TextMeshPro _catchPhrase;
    private List<string> _catchPhrases;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
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
            "Urgh... [DAY]s am I right ?",
            "It's [DAY] my dudes",
            "Since 2020",
            "Do you like the game ?",
            "Looking goood",
            "Hey you"
        };
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
            NavigationService.LoadNextScene(Constants.MainMenuScene);
            return true;
        }
    }
}
