using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingChoiceSceneBhv : SceneBhv
{
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
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
    }

    private void GoToFreePlay()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            Constants.SelectedGameMode = Constants.TrainingFreeGameScene;
            NavigationService.LoadNextScene(Constants.CharSelScene);
            return true;
        }
    }

    private void GoToTrainingDummy()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            Constants.SelectedGameMode = Constants.TrainingDummyGameScene;
            NavigationService.LoadNextScene(Constants.CharSelScene);
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
}
