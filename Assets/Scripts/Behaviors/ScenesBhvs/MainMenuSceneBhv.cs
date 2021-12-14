using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneBhv : SceneBhv
{
    private InputControlerBhv _inputControlerBhv;
    private Run _currentRun;
    private int _menuClickCount;

    private Character debug;

    public override MusicType MusicType => MusicType.Menu;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _inputControlerBhv = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
        _currentRun = PlayerPrefsHelper.GetRun();
        if (_currentRun != null && PlayerPrefsHelper.GetIsInFight())
        {
            ////PlayerPrefsHelper.SaveIsInFight(false);
            ////if (_currentRun.CharacterEncounterAvailability)
            ////    PlayerPrefsHelper.IncrementNumberRunWithoutCharacterEncounter();
            ////PlayerPrefsHelper.ResetRun();
            ////_currentRun = null;
            Cache.InputLocked = true;
            StartCoroutine(Helper.ExecuteAfterDelay(0.1f, () =>
            {
                Instantiator.NewPopupYesNo("In a hurry?", "you've force-quit during a fight. you can continue your current fight, but your opponent will be buffed a bit in order to prevent any abuses.", null, "Damn...", null);
                return false;
            }));
        }
        GameObject.Find("Version").GetComponent<TMPro.TextMeshPro>().text = $"abject\n{Application.version.ToString().ToLower()}";
        SetButtons();
        //var test = debug.SpecialDescription.ToLower();
    }

    private void SetButtons()
    {
        var buttonAscension = GameObject.Find("ButtonNewAscension");
        buttonAscension.GetComponent<ButtonBhv>().EndActionDelegate = GoToNewAscension;
        if (_currentRun != null)
        {
            if (_currentRun.Endless > 0)
                buttonAscension.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Endless Ascension";
            else
                buttonAscension.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Continue Ascension";
        }
        _menuClickCount = 0;
        GameObject.Find("Title").GetComponent<ButtonBhv>().EndActionDelegate = MenuClick;
        GameObject.Find("ButtonTraining").GetComponent<ButtonBhv>().EndActionDelegate = GoToTraining;
        GameObject.Find("ButtonSettings").GetComponent<ButtonBhv>().EndActionDelegate = GoToSettings;
        GameObject.Find("ButtonWannaHelp?").GetComponent<ButtonBhv>().EndActionDelegate = WannaHelp;
        GameObject.Find("ButtonQuit").GetComponent<ButtonBhv>().EndActionDelegate = Quit;
        GameObject.Find("ButtonBug").GetComponent<ButtonBhv>().EndActionDelegate = ReportBug;
        GameObject.Find("ButtonAccount").GetComponent<ButtonBhv>().EndActionDelegate = GoToAccount;
    }

    private void GoToNewAscension()
    {
        if (_currentRun != null && _currentRun.Endless > 0)
        {
            this.Instantiator.NewPopupYesNo("Ascension", "would you like to continue your endless ascension, or to start a new one?", "New", "Continue", (result) =>
            {
                if (!result)
                {
                    _currentRun = null;
                    PlayerPrefsHelper.ResetRun();
                }
                Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
                return result;
            });
        }
        else
            Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);

        object OnBlend(bool result)
        {
            Cache.CurrentGameMode = GameMode.Ascension;
            if (_currentRun != null)
                NavigationService.LoadNextScene(Constants.StepsAscensionScene);
            else
            {
                if (!PlayerPrefsHelper.IsCinematicWatched(Realm.Hell.GetHashCode()))
                    NavigationService.LoadNextScene(Constants.LoreScene, new NavigationParameter() { IntParam0 = Realm.Hell.GetHashCode(), StringParam0 = Constants.CharSelScene});
                else
                    NavigationService.LoadNextScene(Constants.TreeScene);
            }
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

    private void ReportBug()
    {
        LogService.SendLogsIfNecessary();
        Application.OpenURL("https://discord.gg/dJG9KHVhCn");
        //Application.OpenURL("https://abject.itch.io/infidhells/devlog/242745/bug-report");
    }

    private void GoToAccount()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.AccountScene);
            return true;
        }
    }

    private void WannaHelp()
    {
        //Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, "PHILL", "Edam", null, 0);
        Instantiator.NewPopupYesNo("Thanks!", "donations, giving feedback, reporting bugs, and talking about the game around you are some stuff you can do!\n(just don't harass your friends about it too much)", null, "Ok", null);
    }

    private void MenuClick()
    {
        ++_menuClickCount;
        if (_menuClickCount == 69)
        {
            Instantiator.NewPopupYesNo("Nice!", "congratulations!\nyou successfully clicked the game name 69 times!", null, "Ok", null);
        }
    }
}
