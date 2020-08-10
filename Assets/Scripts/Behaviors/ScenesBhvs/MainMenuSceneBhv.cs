using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneBhv : SceneBhv
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
        GameObject.Find("ButtonNewAscension").GetComponent<ButtonBhv>().EndActionDelegate = NotImplemented;
        GameObject.Find("ButtonTraining").GetComponent<ButtonBhv>().EndActionDelegate = GoToTraining;
        GameObject.Find("ButtonSettings").GetComponent<ButtonBhv>().EndActionDelegate = GoToSettings;
        GameObject.Find("ButtonWannaHelp?").GetComponent<ButtonBhv>().EndActionDelegate = NotImplemented;
    }

    private void GoToTraining()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            Constants.SelectedGameMode = Constants.TrainingGameScene;
            NavigationService.LoadNextScene(Constants.CharSelScene);
            return true;
        }
    }

    private void GoToSettings()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsScene);
            return true;
        }
    }

    private void NotImplemented()
    {
        Instantiator.NewPopupYesNo("Not Implemented", "this feature hasn't been implemented yet...\n\ncomming soon!", null, "Oh, ok!", null);
    }
}
