using System.Collections.Generic;
using UnityEngine;

public class SettingsInputsSceneBhv : SceneBhv
{
    public Sprite PcKeyboard;
    public Sprite PcMouse;

    private GameObject _gameplaySelector;
    private GameObject _panelLeft;
    private GameObject _panelRight;
    private GameObject _gameplayChoiceButtons;
    private GameObject _gameplayChoiceSwipes;
    private GameObject _buttonsPanels;
    private GameObject _swipesPanels;
    private GameObject _keyBindingPanelGameplay;
    private GameObject _keyBindingPanelMenu;
    private GameObject _sensitivitySelector;
    private GameObject _menuSelector;
    private SpriteRenderer _horizontalOrientation;
    private InputControlerBhv _inputControlerBhv;
    private List<GameObject> _gameplayButtons;
    private List<KeyCode> _keyBinding;
    private List<KeyCode> _defaultKeyBinding;

    private int _listeningKeeBindingId;
    private GameObject _listeningPopup;

    public override MusicType MusicType => MusicType.Continue;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _gameplaySelector = GameObject.Find("GameplaySelector");
        _panelLeft = GameObject.Find("PanelLeft");
        _panelRight = GameObject.Find("PanelRight");
        _buttonsPanels = GameObject.Find("ButtonsPanels");
        _swipesPanels = GameObject.Find("SwipesPanels");
        _keyBindingPanelGameplay = GameObject.Find("KeyBindingPanelGameplay");
        _keyBindingPanelMenu = GameObject.Find("KeyBindingPanelMenu");
        _sensitivitySelector = GameObject.Find("SensitivitySelector");
        _menuSelector = GameObject.Find("MenuSelector");
        _horizontalOrientation = GameObject.Find("HorizontalOrientation").GetComponent<SpriteRenderer>();
        _gameplayButtons = new List<GameObject>();
        _listeningKeeBindingId = -1;
        _inputControlerBhv = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();

        SetButtons();

        Cache.SetLastEndActionClickedName(PlayerPrefsHelper.GetGameplayChoice() == GameplayChoice.Buttons ? _gameplayChoiceButtons.name : _gameplayChoiceSwipes.name);
        GameplayButtonChoice();

        SetSensitivity(PlayerPrefsHelper.GetTouchSensitivity());

        PanelsVisuals(PlayerPrefsHelper.GetButtonsLeftPanel(), _panelLeft, isLeft: true);
        PanelsVisuals(PlayerPrefsHelper.GetButtonsRightPanel(), _panelRight, isLeft: false);
        SetOrientation(PlayerPrefsHelper.GetOrientation(), init: true);
        _keyBinding = PlayerPrefsHelper.GetKeyBinding();
        _defaultKeyBinding = PlayerPrefsHelper.GetKeyBinding(Constants.PpKeyBindingDefault);
        for (int i = 0; i < _keyBinding.Count; ++i)
            UpdateKeyBindingVisual(i);
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
        (_gameplayChoiceButtons = GameObject.Find("GameplayButtons")).GetComponent<ButtonBhv>().EndActionDelegate = GameplayButtonChoice;
        (_gameplayChoiceSwipes = GameObject.Find("GameplaySwipes")).GetComponent<ButtonBhv>().EndActionDelegate = GameplayButtonChoice;
#if !UNITY_ANDROID
        _gameplayChoiceButtons.GetComponent<SpriteRenderer>().sprite = PcKeyboard;
        _gameplayChoiceSwipes.GetComponent<SpriteRenderer>().sprite = PcMouse;
#endif
        GameObject.Find("SwipeType").GetComponent<ButtonBhv>().EndActionDelegate = FlipGameplaySwipe;
        GameObject.Find("0.25").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(0.25f); };
        GameObject.Find("0.50").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(0.5f); };
        GameObject.Find("1.00").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(1.0f); };
        GameObject.Find("1.50").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(1.5f); };
        GameObject.Find("2.00").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(2.0f); };
        GameObject.Find("2.50").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(2.5f); };
        GameObject.Find("3.00").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetSensitivity(3.0f); };

        _keyBindingPanelGameplay.transform.GetChild(0).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(0); };
        _keyBindingPanelGameplay.transform.GetChild(1).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(1); };
        _keyBindingPanelGameplay.transform.GetChild(2).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(2); };
        _keyBindingPanelGameplay.transform.GetChild(3).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(3); };
        _keyBindingPanelGameplay.transform.GetChild(4).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(4); };
        _keyBindingPanelGameplay.transform.GetChild(5).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(5); };
        _keyBindingPanelGameplay.transform.GetChild(6).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(6); };
        _keyBindingPanelGameplay.transform.GetChild(7).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(7); };
        _keyBindingPanelGameplay.transform.GetChild(8).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(8); };
        _keyBindingPanelGameplay.transform.GetChild(9).GetComponent<ButtonBhv>().EndActionDelegate = () => { SwitchKeyBindingPanels(1); };

        _keyBindingPanelMenu.transform.GetChild(0).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(9); };
        _keyBindingPanelMenu.transform.GetChild(1).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(10); };
        _keyBindingPanelMenu.transform.GetChild(2).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(11); };
        _keyBindingPanelMenu.transform.GetChild(3).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(12); };
        _keyBindingPanelMenu.transform.GetChild(4).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(13); };
        _keyBindingPanelMenu.transform.GetChild(5).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(14); };
        _keyBindingPanelMenu.transform.GetChild(6).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(15); };
        _keyBindingPanelMenu.transform.GetChild(7).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(16); };
        _keyBindingPanelMenu.transform.GetChild(8).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(17); };
        _keyBindingPanelMenu.transform.GetChild(9).GetComponent<ButtonBhv>().EndActionDelegate = () => { SwitchKeyBindingPanels(0); };

        GameObject.Find("Vertical").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetOrientation(Direction.Vertical); };
        GameObject.Find("Horizontal").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetOrientation(Direction.Horizontal); };

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

    private void GameplayButtonChoice()
    {
        var choiceButtonName = Cache.LastEndActionClickedName;
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
#if UNITY_ANDROID
        if (gameplayChoice == GameplayChoice.Buttons)
        {

            _swipesPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _buttonsPanels.GetComponent<PositionBhv>().UpdatePositions();
        }
        else
        {
            _buttonsPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _swipesPanels.GetComponent<PositionBhv>().UpdatePositions();
            var text = _swipesPanels.transform.GetChild(0).Find("TypeText").GetComponent<TMPro.TextMeshPro>();
            var sprite = _swipesPanels.transform.GetChild(0).Find("SwipeType").GetComponent<SpriteRenderer>();
            sprite.enabled = true;
            sprite.GetComponent<BoxCollider2D>().enabled = true;
            text.enabled = true;
            if (gameplayChoice == GameplayChoice.SwipesRightHanded)
            {
                sprite.flipX = false;
                GameObject.Find("GameplaySwipes").GetComponent<SpriteRenderer>().flipX = false;
                text.text = $"Right Handed {Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}(click to change)";
            }
            else
            {
                sprite.flipX = true;
                GameObject.Find("GameplaySwipes").GetComponent<SpriteRenderer>().flipX = true;
                text.text = $"Left Handed {Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}(click to change)";
            }
        }
#else
        if (gameplayChoice == GameplayChoice.Buttons) //KeyBoard
        {
            _swipesPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _buttonsPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelGameplay.GetComponent<PositionBhv>().UpdatePositions();
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
        }
        else //Mouse
        {
            _buttonsPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _swipesPanels.GetComponent<PositionBhv>().UpdatePositions();
            var text = _swipesPanels.transform.GetChild(0).Find("TypeText").GetComponent<TMPro.TextMeshPro>();
            var sprite = _swipesPanels.transform.GetChild(0).Find("SwipeType").GetComponent<SpriteRenderer>();
            sprite.enabled = false;
            sprite.GetComponent<BoxCollider2D>().enabled = false;
            text.enabled = false;
            text.gameObject.transform.Find("Overline").GetComponent<SpriteRenderer>().enabled = false;
        }
#endif
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

    private void SetKeyBinding(int id)
    {
        StartCoroutine(Helper.ExecuteAfterDelay(0.25f, () => { _listeningKeeBindingId = id; }));
        Cache.EscapeLocked = true;
        var sonicDropExplanation =  id == (int)KeyBinding.SonicDrop ? $"\n\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}if unset, you can sonic drop\r\nby tapping{Constants.MaterialEnd} soft drop {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}twice" : "";
        _listeningPopup = Instantiator.NewPopupYesNo("Set Key", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}set new key for: {Constants.MaterialEnd}{((KeyBinding)id).GetDescription().ToLower()}{sonicDropExplanation}", "Cancel", "Default", OnSetKey);

        void OnSetKey(bool result)
        {
            if (result)
            {
                CheckAlreadyKeyBinding(_defaultKeyBinding[_listeningKeeBindingId], _listeningKeeBindingId);
                _keyBinding[_listeningKeeBindingId] = _defaultKeyBinding[_listeningKeeBindingId];
                PlayerPrefsHelper.SaveKeyBinding(_keyBinding);
                _inputControlerBhv.GetKeyBinding();
                UpdateKeyBindingVisual(_listeningKeeBindingId);
            }
            _listeningKeeBindingId = -1;
            Invoke(nameof(UnlockEscape), 0.25f);
        }
    }

    private void UnlockEscape() { Cache.EscapeLocked = false; }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.rawType == EventType.KeyUp && _listeningKeeBindingId >= 0)
        {
            if (_listeningKeeBindingId == 14 && e.keyCode == _keyBinding[14])
                return;
            //Debug.Log("Detected key code: " + e.keyCode);
            CheckAlreadyKeyBinding(e.keyCode, _listeningKeeBindingId);
            _keyBinding[_listeningKeeBindingId] = e.keyCode;
            PlayerPrefsHelper.SaveKeyBinding(_keyBinding);
            _inputControlerBhv.GetKeyBinding();
            UpdateKeyBindingVisual(_listeningKeeBindingId);
            _listeningPopup.GetComponent<PopupBhv>().ExitPopup();
            _listeningKeeBindingId = -1;
            Invoke(nameof(UnlockEscape), 0.25f);
        }
    }

    private void CheckAlreadyKeyBinding(KeyCode code, int keyId)
    {
        //Check for menu controls
        if (keyId >= (int)KeyBinding.MenuUp && keyId <= (int)KeyBinding.Restart)
        {
            for (int i = 10; i <= 16; ++i)
            {
                if (_keyBinding[i] == code)
                {
                    _keyBinding[i] = KeyCode.None;
                    UpdateKeyBindingVisual(i);
                }
            }
            return;
        }
        //Check for gameplay + escape
        for (int i = (int)KeyBinding.HardDrop; i <= (int)KeyBinding.SonicDrop; ++i)
        {
            if (_keyBinding[i] == code)
            {
                _keyBinding[i] = KeyCode.None;
                UpdateKeyBindingVisual(i);
            }
        }
    }

    private void UpdateKeyBindingVisual(int id)
    {
        TMPro.TextMeshPro tmPro = null;
        if (id < 9)
            tmPro = _keyBindingPanelGameplay.transform.GetChild(id).GetComponent<TMPro.TextMeshPro>();
        else
            tmPro = _keyBindingPanelMenu.transform.GetChild(id - 9).GetComponent<TMPro.TextMeshPro>();
        var separatorId = tmPro.text.IndexOf(Constants.MaterialEnd) + Constants.MaterialEnd.Length;
        var tmpText = tmPro.text.Substring(0, separatorId);
        tmPro.text = $"{tmpText}\n{(_keyBinding[id] == KeyCode.None ? Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32) : "")}{_keyBinding[id]}";
    }

    private void SwitchKeyBindingPanels(int idPanel)
    {
        if (idPanel == 0)
        {
            _keyBindingPanelGameplay.GetComponent<PositionBhv>().UpdatePositions();
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            if (!Cache.OnlyMouseInMenu)
                InvokeNextFrame(() =>
                {
                    _menuSelector.GetComponent<MenuSelectorBhv>().MoveTo(_keyBindingPanelGameplay.transform.GetChild(9).gameObject, true);
                });
        }
        else
        {
            _keyBindingPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelMenu.GetComponent<PositionBhv>().UpdatePositions();
            if (!Cache.OnlyMouseInMenu)
                InvokeNextFrame(() =>
                {
                    _menuSelector.GetComponent<MenuSelectorBhv>().MoveTo(_keyBindingPanelMenu.transform.GetChild(9).gameObject, true);
                });
        }
    }

    private void SetGameplayButtonOnClick()
    {
        var addButton = GameObject.Find(Cache.LastEndActionClickedName);
        var buttonId = int.Parse(addButton.gameObject.name.Substring(addButton.gameObject.name.Length - 2, 2));
        Instantiator.NewPopupGameplayButtons(AfterPopup);
        void AfterPopup(bool result)
        {
            if (!result)
                return;
            var gameplayButtonName = Cache.LastEndActionClickedName;
            SetGameplayButton(addButton, buttonId, gameplayButtonName);
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
        var gameplayButton = GameObject.Find(Cache.LastEndActionClickedName);
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
                    break;
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

    private void SetOrientation(Direction orientation, bool init = false)
    {
        var combatOrientationSelector = GameObject.Find("CombatOrientationSelector");
        combatOrientationSelector.transform.position = new Vector3(GameObject.Find(orientation.ToString()).transform.position.x, combatOrientationSelector.transform.position.y, 0.0f);
        if (orientation == Direction.Horizontal)
            _horizontalOrientation.enabled = true;
        else
            _horizontalOrientation.enabled = false;
        if (!init)
        {
            if (orientation == Direction.Horizontal && PlayerPrefsHelper.GetOrientation() == Direction.Horizontal)
            {
                var oldHorizontalOrientation = PlayerPrefsHelper.GetHorizontalOrientation();
                var newHorizontalOrientation = oldHorizontalOrientation == Direction.Right ? Direction.Left : Direction.Right;
                PlayerPrefsHelper.SaveHorizontalOrientation(newHorizontalOrientation);
                _horizontalOrientation.flipX = newHorizontalOrientation == Direction.Left;
                Cache.HorizontalCameraInitialRotation = null;
            } 
            PlayerPrefsHelper.SaveOrientation(orientation);
        }
        else
            _horizontalOrientation.flipX = PlayerPrefsHelper.GetHorizontalOrientation() == Direction.Left;
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
            //Debug.Log("\t[DEBUG]\tverif = " + verif);
            Instantiator.NewPopupYesNo("Caution", "you are missing some needed buttons!", null, "Oh indeed!", null);
        }
        else
        {
            Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
            bool OnBlend(bool result)
            {
                NavigationService.LoadPreviousScene();
                return true;
            }
        }
    }

    private void ResetDefault()
    {
        Instantiator.NewPopupYesNo("Default", "are you willing to restore the default settings ?", "Nope", "Yup", OnDefault);
        void OnDefault(bool result)
        {
            if (!result)
                return;
            PlayerPrefsHelper.SaveGhostColor(Constants.PpGhostPieceColorDefault);
            PlayerPrefsHelper.SaveGameplayChoice(Constants.PpGameplayChoiceDefault);
            PlayerPrefsHelper.SaveButtonsLeftPanel(Constants.PpButtonsLeftPanelDefault);
            PlayerPrefsHelper.SaveButtonsRightPanel(Constants.PpButtonsRightPanelDefault);
            PlayerPrefsHelper.SaveKeyBinding(_defaultKeyBinding);
            _inputControlerBhv.GetKeyBinding();
            foreach (var gm in _gameplayButtons)
                Destroy(gm);
            _gameplayButtons.Clear();
            PanelButtonsVisibility(_panelLeft, _gameplayButtons);
            PanelButtonsVisibility(_panelRight, _gameplayButtons);
            Init();
        }
    }
}
