using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoEndSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.None;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        GameObject.Find("ButtonPositive").GetComponent<ButtonBhv>().EndActionDelegate = GoBackToMainMenu;
    }

    private void GoBackToMainMenu()
    {
        NavigationService.LoadBackUntil(Constants.MainMenuScene);
    }
}
