using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneBhv : SceneBhv
{
    private InputControlerBhv _inputControlerBhv;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _inputControlerBhv = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
        if (PlayerPrefsHelper.GetRun() != null && PlayerPrefsHelper.GetIsInFight() == true)
        {
            PlayerPrefsHelper.SaveIsInFight(false);
            PlayerPrefsHelper.ResetRun();
            Constants.InputLocked = true;
            StartCoroutine(Helper.ExecuteAfterDelay(0.25f, () =>
            {
                Instantiator.NewPopupYesNo("Sorry...", "you've force-quit during a fight. therefore, your progress has been deleted...", null, "Damn...", null);
                return false;
            }));
        }
        GameObject.Find("Version").GetComponent<TMPro.TextMeshPro>().text = $"abject\n{Application.version.ToString().ToLower()}";
        SetButtons();
    }

    private void SetButtons()
    {
        var buttonAscension = GameObject.Find("ButtonNewAscension");
        buttonAscension.GetComponent<ButtonBhv>().EndActionDelegate = GoToNewAscension;
        if (PlayerPrefsHelper.GetRun() != null)
            buttonAscension.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Continue Ascension";
        GameObject.Find("ButtonTraining").GetComponent<ButtonBhv>().EndActionDelegate = GoToTraining;
        GameObject.Find("ButtonSettings").GetComponent<ButtonBhv>().EndActionDelegate = GoToSettings;
        GameObject.Find("ButtonWannaHelp?").GetComponent<ButtonBhv>().EndActionDelegate = NotImplemented;
        GameObject.Find("ButtonQuit").GetComponent<ButtonBhv>().EndActionDelegate = Quit;
    }

    private void GoToNewAscension()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            Constants.CurrentGameMode = GameMode.Ascension;
            if (PlayerPrefsHelper.GetRun() != null)
                NavigationService.LoadNextScene(Constants.StepsAscensionScene);
            else
                NavigationService.LoadNextScene(Constants.CharSelScene);
            return true;
        }
    }

    private void GoToTraining()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.TrainingChoiceScene);
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

    private void Quit()
    {
        Instantiator.NewPopupYesNo("Quit", "do you wish to quit the game?", "Nope", "Yup", OnQuit);
        object OnQuit(bool result)
        {
#if !UNITY_ANDROID
            _inputControlerBhv.MenuSelector.Reset();
#endif
            if (result)
                Application.Quit();
            return result;
        }
    }

    private void NotImplemented()
    {
        Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, "Baphomeh", "Edam", null, 0);
        //Instantiator.NewPopupYesNo("Not Implemented", "this feature hasn't been implemented yet...\n\ncomming soon!", null, "Oh, ok!", null);
    }
}
