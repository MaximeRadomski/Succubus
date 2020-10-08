using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsGameplaySceneBhv : SceneBhv
{
    private GameObject _ghostSelector;
    private GameObject _gameplaySelector;
    private GameObject _panelLeft;
    private GameObject _panelRight;
    private GameObject _gameplayChoiceButtons;
    private GameObject _gameplayChoiceSwipes;
    private GameObject _buttonsPanels;
    private GameObject _swipesPanels;
    private GameObject _sensitivitySelector;
    private List<GameObject> _gameplayButtons;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _ghostSelector = GameObject.Find("GhostSelector");
        _gameplaySelector = GameObject.Find("GameplaySelector");
        _panelLeft = GameObject.Find("PanelLeft");
        _panelRight = GameObject.Find("PanelRight");
        _buttonsPanels = GameObject.Find("ButtonsPanels");
        _swipesPanels = GameObject.Find("SwipesPanels");
        _sensitivitySelector = GameObject.Find("SensitivitySelector");
        _gameplayButtons = new List<GameObject>();
        SetButtons();

        Constants.SetLastEndActionClickedName(PlayerPrefsHelper.GetGhostColor());
        GhostColorChoice();

        Constants.SetLastEndActionClickedName(PlayerPrefsHelper.GetGameplayChoice() == GameplayChoice.Buttons ? _gameplayChoiceButtons.name : _gameplayChoiceSwipes.name);
        GameplayButtonChoice();

        SetSensitivity(PlayerPrefsHelper.GetTouchSensitivity());

        PanelsVisuals(PlayerPrefsHelper.GetButtonsLeftPanel(), _panelLeft, isLeft:true);
        PanelsVisuals(PlayerPrefsHelper.GetButtonsRightPanel(), _panelRight, isLeft:false);
    }

    private void PanelsVisuals(string panelStr, GameObject panel, bool isLeft)
    {
        for (int i = 0; i < panelStr.Length; ++i)
        {
            if (panelStr[i] == '0')
                continue;
            SetGameplayButton(GameObject.Find((isLeft ? "L-" : "R-") + "Add" + i.ToString("D2")), i, Helper.LetterToGameplayButton(panelStr[i]));
        }

        PanelButtonsVisibility(panel, _gameplayButtons);
    }

    private void SetButtons()
    {
        GameObject.Find("Ghost1").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;
        GameObject.Find("Ghost2").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;
        GameObject.Find("Ghost3").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;
        GameObject.Find("Ghost4").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;
        GameObject.Find("Ghost5").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;

        (_gameplayChoiceButtons = GameObject.Find("GameplayButtons")).GetComponent<ButtonBhv>().EndActionDelegate = GameplayButtonChoice;
        (_gameplayChoiceSwipes = GameObject.Find("GameplaySwipes")).GetComponent<ButtonBhv>().EndActionDelegate = GameplayButtonChoice;
        GameObject.Find("SwipeType").GetComponent<ButtonBhv>().EndActionDelegate = FlipGameplaySwipe;
        GameObject.Find("0.25").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(0.25f); };
        GameObject.Find("0.50").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(0.5f); };
        GameObject.Find("1.00").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(1.0f); };
        GameObject.Find("1.50").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(1.5f); };
        GameObject.Find("2.00").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(2.0f); };
        GameObject.Find("2.50").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(2.5f); };
        GameObject.Find("3.00").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(3.0f); };

        SetPanelButton(GameObject.Find("PanelLeft"));
        SetPanelButton(GameObject.Find("PanelRight"));

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        GameObject.Find("ButtonReset").GetComponent<ButtonBhv>().EndActionDelegate = ResetDefault;
    }

    private void SetPanelButton(GameObject panel)
    {
        foreach (Transform child in panel.transform)
        {
            child.gameObject.GetComponent<ButtonBhv>().EndActionDelegate = SetGameplayButtonOnClick;
            child.transform.SetParent(panel.transform);
        }
    }

    private void GhostColorChoice()
    {
        var choiceId = Constants.LastEndActionClickedName.Substring(Constants.LastEndActionClickedName.Length - 1, 1);
        var choiceGameObject = GameObject.Find("Ghost" + choiceId);
        _ghostSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _ghostSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveGhostColor(choiceId);
    }

    private void GameplayButtonChoice()
    {
        var choiceButtonName = Constants.LastEndActionClickedName;
        var choiceGameObject = GameObject.Find(choiceButtonName);

        var choiceType = GameplayChoice.Buttons;
        if (!choiceButtonName.Contains("Buttons"))
        {
            if (PlayerPrefsHelper.GetGameplayChoice() == GameplayChoice.SwipesLeftHanded)
                choiceType = GameplayChoice.SwipesLeftHanded;
            else
                choiceType = GameplayChoice.SwipesRightHanded;
        }
        _gameplaySelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _gameplaySelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveGameplayChoice(choiceType);
        UpdateViewFromGameplayChoice(choiceType);
    }

    private void UpdateViewFromGameplayChoice(GameplayChoice gameplayChoice)
    {
        if (gameplayChoice == GameplayChoice.Buttons)
        {
            _swipesPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _buttonsPanels.GetComponent<PositionBhv>().UpdatePositions();
        }
        else
        {
            _buttonsPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _swipesPanels.GetComponent<PositionBhv>().UpdatePositions();
            var text = _swipesPanels.transform.GetChild(0).Find("TypeText").GetComponent<TMPro.TextMeshPro>();
            var sprite = _swipesPanels.transform.GetChild(0).Find("SwipeType").GetComponent<SpriteRenderer>();
            if (gameplayChoice == GameplayChoice.SwipesRightHanded)
            {
                sprite.flipX = false;
                GameObject.Find("GameplaySwipes").GetComponent<SpriteRenderer>().flipX = false;
                text.text = $"Right Handed {Constants.MaterialLong_3_2}(click to change)";
            }
            else
            {
                sprite.flipX = true;
                GameObject.Find("GameplaySwipes").GetComponent<SpriteRenderer>().flipX = true;
                text.text = $"Left Handed {Constants.MaterialLong_3_2}(click to change)";
            }
        }
    }

    private void FlipGameplaySwipe()
    {
        var currentGameplayType = PlayerPrefsHelper.GetGameplayChoice();
        var newGameplayStyle = currentGameplayType == GameplayChoice.SwipesRightHanded ? GameplayChoice.SwipesLeftHanded : GameplayChoice.SwipesRightHanded;
        PlayerPrefsHelper.SaveGameplayChoice(newGameplayStyle);
        UpdateViewFromGameplayChoice(newGameplayStyle);
    }

    private void SetSensitivity(float amount)
    {
        var buttonName = amount.ToString("0.00").Replace(",", ".");
        var buttonTapped = GameObject.Find(buttonName);
        _sensitivitySelector.transform.position = buttonTapped.transform.position;
        PlayerPrefsHelper.SaveTouchSensitivity(amount);
    }

    private void SetGameplayButtonOnClick()
    {
        var addButton = GameObject.Find(Constants.LastEndActionClickedName);
        var buttonId = int.Parse(addButton.gameObject.name.Substring(addButton.gameObject.name.Length - 2, 2));
        Instantiator.NewPopupGameplayButtons(AfterPopup);
        object AfterPopup(bool result)
        {
            if (!result)
                return result;
            var gameplayButtonName = Constants.LastEndActionClickedName;
            SetGameplayButton(addButton, buttonId, gameplayButtonName);
            return result;
        }
    }

    private void SetGameplayButton(GameObject addButton, int buttonId, string gameplayButtonName)
    {
        //Debug.Log("\t[DEBUG]\tgameplayButtonName = " + gameplayButtonName);
        var gameplayButton = Instantiator.NewGameplayButton(gameplayButtonName, addButton.transform.position);
        gameplayButton.GetComponent<ButtonBhv>().EndActionDelegate = UnsetGameplayButtonPosition;
        if (addButton.gameObject.name[0] == 'L')
        {
            _gameplayButtons.Add(gameplayButton);
            gameplayButton.name = gameplayButton.name + Helper.DoesListContainsSameFromName(_gameplayButtons, gameplayButton.name).ToString("D2");
            PanelButtonsVisibility(_panelLeft, _gameplayButtons);
            var save = PlayerPrefsHelper.GetButtonsLeftPanel();
            save = save.ReplaceChar(buttonId, Helper.GameplayButtonToLetter(gameplayButtonName));
            PlayerPrefsHelper.SaveButtonsLeftPanel(save);
            gameplayButton.transform.SetParent(_panelLeft.transform);
        }
        else
        {
            _gameplayButtons.Add(gameplayButton);
            gameplayButton.name = gameplayButton.name + Helper.DoesListContainsSameFromName(_gameplayButtons, gameplayButton.name).ToString("D2");
            PanelButtonsVisibility(_panelRight, _gameplayButtons);
            var save = PlayerPrefsHelper.GetButtonsRightPanel();
            save = save.ReplaceChar(buttonId, Helper.GameplayButtonToLetter(gameplayButtonName));
            PlayerPrefsHelper.SaveButtonsRightPanel(save);
            gameplayButton.transform.SetParent(_panelRight.transform);
        }
    }

    private void ListRemoveFromName(List<GameObject> list, string name)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].gameObject.name == name)
            {
                list.RemoveAt(i);
                return;
            }
        }
    }

    private void UnsetGameplayButtonPosition()
    {
        var gameplayButton = GameObject.Find(Constants.LastEndActionClickedName);
        var isLeft = gameplayButton.transform.position.x < 0;
        if (isLeft)
        {
            ListRemoveFromName(_gameplayButtons, gameplayButton.name);
            foreach (Transform child in _panelLeft.transform)
            {
                if (Helper.VectorEqualsPrecision(child.position, gameplayButton.transform.position, 0.05f))
                {
                    var childId = int.Parse(child.gameObject.name.Substring(child.gameObject.name.Length - 2, 2));
                    var save = PlayerPrefsHelper.GetButtonsLeftPanel();
                    save = save.ReplaceChar(childId, '0');
                    PlayerPrefsHelper.SaveButtonsLeftPanel(save);
                }
            }
            PanelButtonsVisibility(_panelLeft, _gameplayButtons);
        }            
        else
        {
            ListRemoveFromName(_gameplayButtons, gameplayButton.name);
            foreach (Transform child in _panelRight.transform)
            {
                if (Helper.VectorEqualsPrecision(child.position, gameplayButton.transform.position, 0.05f))
                {
                    var childId = int.Parse(child.gameObject.name.Substring(child.gameObject.name.Length - 2, 2));
                    var save = PlayerPrefsHelper.GetButtonsRightPanel();
                    save = save.ReplaceChar(childId, '0');
                    PlayerPrefsHelper.SaveButtonsRightPanel(save);
                }
            }
            PanelButtonsVisibility(_panelRight, _gameplayButtons);
        }
        Destroy(gameplayButton);
    }

    private void PanelButtonsVisibility(GameObject panel, List<GameObject> gameplayButtons)
    {
        foreach (Transform addButton in panel.transform)
        {
            addButton.gameObject.SetActive(true);
        }
        foreach (Transform addButton in panel.transform)
        {
            if (!addButton.name.Contains("Add"))
                continue;
            foreach (var gameplayButton in gameplayButtons)
            {
                if (gameplayButton != null && Vector3.Distance(addButton.position, gameplayButton.transform.position) <= 3.0f)
                    addButton.gameObject.SetActive(false);
            }
        }
    }

    public override void PauseOrPrevious()
    {
        GoToPrevious();
    }

    private void GoToPrevious()
    {
        var verif = "000000000";
        foreach (GameObject gm in _gameplayButtons)
        {
            if (gm.name.Contains(Constants.GoButtonLeftName))
                verif = verif.ReplaceChar(0, '1');
            else if (gm.name.Contains(Constants.GoButtonRightName))
                verif = verif.ReplaceChar(1, '1');
            else if (gm.name.Contains(Constants.GoButtonDownName))
                verif = verif.ReplaceChar(2, '1');
            else if (gm.name.Contains(Constants.GoButtonDropName))
                verif = verif.ReplaceChar(3, '1');
            else if (gm.name.Contains(Constants.GoButtonHoldName))
                verif = verif.ReplaceChar(4, '1');
            else if (gm.name.Contains(Constants.GoButtonAntiClockName))
                verif = verif.ReplaceChar(5, '1');
            else if (gm.name.Contains(Constants.GoButtonClockName))
                verif = verif.ReplaceChar(6, '1');
            else if (gm.name.Contains(Constants.GoButtonItemName))
                verif = verif.ReplaceChar(7, '1');
            else if (gm.name.Contains(Constants.GoButtonSpecialName))
                verif = verif.ReplaceChar(8, '1');
        }
        if (verif.Contains("0"))
        {
            Debug.Log("\t[DEBUG]\tverif = " + verif);
            Instantiator.NewPopupYesNo("Caution", "you are missing some needed buttons!", null, "Oh indeed!", null);
        }
        else
        {
            Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse:true);
            object OnBlend(bool result)
            {
                NavigationService.LoadPreviousScene();
                return true;
            }
        }
    }

    private void ResetDefault()
    {
        Instantiator.NewPopupYesNo("Default", "are you willing to restore the default settings ?", "Nope", "Yup", OnDefault);
        object OnDefault(bool result)
        {
            if (!result)
                return result;
            PlayerPrefsHelper.SaveGhostColor(Constants.PpGhostPieceColorDefault);
            PlayerPrefsHelper.SaveGameplayChoice(Constants.PpGameplayChoiceDefault);
            PlayerPrefsHelper.SaveButtonsLeftPanel(Constants.PpButtonsLeftPanelDefault);
            PlayerPrefsHelper.SaveButtonsRightPanel(Constants.PpButtonsRightPanelDefault);
            foreach (var gm in _gameplayButtons)
                Destroy(gm);
            _gameplayButtons.Clear();
            PanelButtonsVisibility(_panelLeft, _gameplayButtons);
            PanelButtonsVisibility(_panelRight, _gameplayButtons);
            Init();
            return result;
        }
    }
}
