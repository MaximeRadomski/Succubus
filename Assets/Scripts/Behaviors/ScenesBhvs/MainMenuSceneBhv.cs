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
        GameObject.Find("ButtonSettings").GetComponent<ButtonBhv>().EndActionDelegate = NotImplemented;
        GameObject.Find("ButtonWannaHelp?").GetComponent<ButtonBhv>().EndActionDelegate = NotImplemented;
    }

    private void GoToTraining()
    {
        NavigationService.LoadNextScene(Constants.TrainingGameScene);
    }

    private void NotImplemented()
    {
        Instantiator.NewPopupYesNo("Not Implemented", "this feature hasn't been implemented yet...\n\ncomming soon!", null, "Oh, ok!", null);
    }
}
