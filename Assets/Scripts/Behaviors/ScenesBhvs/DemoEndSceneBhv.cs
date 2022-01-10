using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (SceneManager.GetActiveScene().name == Constants.EndScene)
            PlayerPrefsHelper.ResetCinematicsWatched();
        GameObject.Find("ButtonPositive").GetComponent<ButtonBhv>().EndActionDelegate = GoToGamOver;
    }

    private void GoToGamOver()
    {
        NavigationService.LoadNextScene(Constants.GameOverScene);
    }
}
