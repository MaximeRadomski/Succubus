using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSceneBhv : SceneBhv
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
        GameObject.Find("ButtonAudioSettings").GetComponent<ButtonBhv>().EndActionDelegate = GoToAudioSettings;
        GameObject.Find("ButtonGameplaySettings").GetComponent<ButtonBhv>().EndActionDelegate = GoToGameplaySettings;
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
    }

    private void GoToAudioSettings()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsAudioScene);
            return true;
        }
    }

    private void GoToGameplaySettings()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsGameplayScene);
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
