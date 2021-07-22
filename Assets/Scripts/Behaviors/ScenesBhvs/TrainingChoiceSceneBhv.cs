using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingChoiceSceneBhv : SceneBhv
{
    private GameObject _buttonHighScores;

    public override MusicType MusicType => MusicType.Menu;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        SetButtons();
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonFreePlay").GetComponent<ButtonBhv>().EndActionDelegate = GoToFreePlay;
        GameObject.Find("ButtonTrainingDummy").GetComponent<ButtonBhv>().EndActionDelegate = GoToTrainingDummy;
        GameObject.Find("ButtonOldSchool").GetComponent<ButtonBhv>().EndActionDelegate = GoToOldSchool;
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        (_buttonHighScores = GameObject.Find("ButtonHighScores")).GetComponent<ButtonBhv>().EndActionDelegate = GoToButtonHighScores;
        if (PlayerPrefsHelper.GetTrainingHighScoreHistory().Count <= 0)
            _buttonHighScores.SetActive(false);
    }

    private void GoToFreePlay()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            Constants.CurrentGameMode = GameMode.TrainingFree;
            NavigationService.LoadNextScene(Constants.CharSelScene);
            return true;
        }
    }

    private void GoToTrainingDummy()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            Constants.CurrentGameMode = GameMode.TrainingDummy;
            NavigationService.LoadNextScene(Constants.CharSelScene);
            return true;
        }
    }

    private void GoToOldSchool()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            Constants.CurrentGameMode = GameMode.TrainingOldSchool;
            NavigationService.LoadNextScene(Constants.TrainingFreeGameScene);
            return true;
        }
    }

    private void GoToPrevious()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        object OnBlend(bool result)
        {
            NavigationService.LoadPreviousScene();
            return true;
        }
    }

    private void GoToButtonHighScores()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.HighScoreScene);
            return true;
        }
    }
}
