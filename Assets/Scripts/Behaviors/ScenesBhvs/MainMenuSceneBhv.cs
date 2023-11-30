using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneBhv : SceneBhv
{
    private InputControlerBhv _inputControlerBhv;
    private Run _currentRun;
    private int _menuClickCount;

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
        if (_currentRun != null && PlayerPrefsHelper.GetIsInFight() && Cache.InAHurryPopup == false)
        {
            ////PlayerPrefsHelper.SaveIsInFight(false);
            ////if (_currentRun.CharacterEncounterAvailability)
            ////    PlayerPrefsHelper.IncrementNumberRunWithoutCharacterEncounter();
            ////PlayerPrefsHelper.ResetRun();
            ////_currentRun = null;
            Cache.InputLocked = true;
            StartCoroutine(Helper.ExecuteAfterDelay(0.1f, () =>
            {
                Cache.InAHurryPopup = true;
                Instantiator.NewPopupYesNo("In a hurry?", "you've force-quit during a fight. you can continue your current fight, but your opponent will be buffed a bit in order to prevent any abuses.", null, "Damn...", null);
            }));
        }
        GameObject.Find("Version").GetComponent<TMPro.TextMeshPro>().text = $"abject\n{Application.version.ToString().ToLower()}";
        SetButtons();
    }

    private void SetButtons()
    {
        var buttonAscension = GameObject.Find("ButtonNewAscension");
        buttonAscension.GetComponent<ButtonBhv>().EndActionDelegate = GoToNewAscension;
        if (_currentRun != null)
        {
            if (_currentRun.Endless > 0)
            {
                buttonAscension.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Endless Ascension";
                buttonAscension.transform.GetChild(1).localScale = new Vector3(7.742f, 1, 1);
            }
            else
            {
                buttonAscension.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Continue Ascension";
                buttonAscension.transform.GetChild(0).localPosition = new Vector3(0.038f, 0.326f, 0);
                buttonAscension.transform.GetChild(1).localScale = new Vector3(8.250f, 1, 1);
                buttonAscension.transform.GetChild(1).localPosition = new Vector3(-0.072f, 0, 0);
            }
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
            Helper.ReinitKeyboardInputs(this);
            this.Instantiator.NewPopupYesNo("Ascension", $"would you like to continue your endless ascension, or to start a new one?\n\ndifficulty: {(_currentRun.Difficulty - 1).ToString().ToLower()} -}} {_currentRun.Difficulty.ToString().ToLower()}", "New", "Continue", (result) =>
            {
                if (!result)
                {
                    _currentRun = null;
                    PlayerPrefsHelper.ResetRun();
                }
                Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
            });
        }
        else
        {
            if (!PlayerPrefsHelper.GetRhythmAttacksPopupSeen())
            {
                this.Instantiator.NewPopupYesNo("Accessibility", $"some opponents have a rhythm dependant attack.\nif you play without sound, an accessibility option replaces this attack by one not needing any sound in the settings.", null, "Noted!", (result) =>
                {
                    PlayerPrefsHelper.SaveRhythmAttacksPopupSeen(true);
                    Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
                });
            }
            else
                Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        }

        bool OnBlend(bool result)
        {
            Cache.CurrentGameMode = GameMode.Ascension;
            if (_currentRun != null)
            {
                if (!PlayerPrefsHelper.IsCinematicWatched((int)Realm.Hell))
                    NavigationService.LoadNextScene(Constants.LoreScene, new NavigationParameter() { IntParam0 = (int)Realm.Hell, StringParam0 = Constants.StepsAscensionScene });
                else
                    NavigationService.LoadNextScene(Constants.StepsAscensionScene);
            }
            else
            {
                if (!PlayerPrefsHelper.IsCinematicWatched((int)Realm.Hell))
                {
                    var nextScene = Constants.CharSelScene;
                    var resources = PlayerPrefsHelper.GetTotalResources();
                    var totalResources = 0;
                    foreach (var resource in resources)
                        totalResources += resource;
                    if (PlayerPrefsHelper.GetBoughtTreeNodes() != "" || totalResources > 0)
                        nextScene = Constants.TreeScene;
                    NavigationService.LoadNextScene(Constants.LoreScene, new NavigationParameter() { IntParam0 = (int)Realm.Hell, StringParam0 = nextScene });
                }
                else
                    NavigationService.LoadNextScene(Constants.TreeScene);
            }
            return true;
        }
    }

    private void GoToTraining()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.TrainingChoiceScene);
            return true;
        }
    }

    private void GoToSettings()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsScene);
            return true;
        }
    }

    private void Quit()
    {
        Instantiator.NewPopupYesNo("Quit", "do you wish to quit the game?", "Nope", "Yup", OnQuit);
        void OnQuit(bool result)
        {
#if !UNITY_ANDROID
            _inputControlerBhv.MenuSelector.Reset();
#endif
            if (result)
                Application.Quit();
        }
    }

    private void ReportBug()
    {
        LogService.TrySendLogs();
        Application.OpenURL("https://discord.gg/dJG9KHVhCn");
        //Application.OpenURL("https://abject.itch.io/infidhells/devlog/242745/bug-report");
    }

    private void GoToAccount()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.AccountScene);
            return true;
        }
    }

    private void WannaHelp()
    {
        //Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, "PHILL", "Edam", null, 0);
        Instantiator.NewPopupYesNo("Thanks!", $"donations, giving feedback, reporting bugs, and talking about the game around you are some stuff you can do to help!\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}but yeah donations might be what i suggest the most. definitely.", null, "Ok", null);
    }

    private void MenuClick()
    {
        ++_menuClickCount;
        if (_menuClickCount == 2)
            Instantiator.NewPopupYesNo("Yes.", "yes, this is clickable.", null, "Ok", null);
        else if (_menuClickCount == 10)
            Instantiator.NewPopupYesNo("Hmmm...", "you sure like to click don't you?", null, "Ok", null);
        else if (_menuClickCount == 50)
            Instantiator.NewPopupYesNo("50 !", "50 cliks!\nyou know there is nothing to unlock by clicking this right ?", null, "Ok", null);
        else if (_menuClickCount == 69)
            Instantiator.NewPopupYesNo("Nice!", "congratulations!\nyou successfully clicked the game name 69 times!", null, "Ok", null);
        else if (_menuClickCount == 100)
            Instantiator.NewPopupYesNo("100 !", "100 clicks!\nthis is the last message about clicks tho.\nplease stop for your own good.", null, "Ok", null);
        else if (_menuClickCount == 150)
            Instantiator.NewPopupYesNo("...", "are you for real?", null, "Ok", null);
        else if (_menuClickCount == 200)
            Instantiator.NewPopupYesNo("!!!", "you've got to be kidding me!", null, "Ok", null);
        else if (_menuClickCount == 300)
            Instantiator.NewPopupYesNo("???", "stop it god damnit!", null, "Ok", null);
        else if (_menuClickCount == 666)
        {
            Instantiator.NewPopupYesNo("666", "666 clicks!\nall skins are now unlocked.", null, "Ok", null);
            PlayerPrefsHelper.SaveUnlockedSkins("111111111111111111111111111111111111111111111111");
        }
        else if (_menuClickCount > 666 && _menuClickCount < 670)
            Instantiator.NewPopupYesNo("Stop.", "you can stop now.", null, "Ok", null);
        else if (_menuClickCount == 670)
            Instantiator.NewPopupYesNo("You...", "you're a bit dense aren't you?", null, "Ok", null);
    }
}
