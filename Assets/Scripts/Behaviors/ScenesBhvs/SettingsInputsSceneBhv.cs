using System;
using System.Collections.Generic;
using System.Linq;
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
    private GameObject _gameplayChoiceController;
    private GameObject _buttonsPanels;
    private GameObject _swipesPanels;
    private GameObject _keyBindingPanelGameplay;
    private GameObject _keyBindingPanelMenu;
    private GameObject _controllerPanelGameplay;
    private GameObject _controllerPanelMenu;
    private GameObject _controllerPresetsContainer;
    private GameObject _controllerPresetsSelector;
    private GameObject _sensitivitySelector;
    private GameObject _menuSelector;
    private SpriteRenderer _horizontalOrientation;
    private InputControlerBhv _inputControlerBhv;
    private List<GameObject> _gameplayButtons;
    private List<KeyCode> _keyBinding;
    private List<KeyCode> _defaultKeyBinding;
    private List<JoystickInput> _controllerBinding;
    private List<JoystickInput> _defaultControllerBinding;

    private int _listeningKeyBindingId;
    private int _listeningControllerBindingId;
    private int _listeningAxisFirstPass;
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
        _controllerPanelGameplay = GameObject.Find("ControllerPanelGameplay");
        _controllerPanelMenu = GameObject.Find("ControllerPanelMenu");
        _controllerPresetsContainer = GameObject.Find("ControllerPresetsContainer");
        _controllerPresetsSelector = GameObject.Find("PresetSelector");
        _sensitivitySelector = GameObject.Find("SensitivitySelector");
        _menuSelector = GameObject.Find("MenuSelector");
        _horizontalOrientation = GameObject.Find("HorizontalOrientation").GetComponent<SpriteRenderer>();
        _gameplayButtons = new List<GameObject>();
        _listeningKeyBindingId = -1;
        _listeningControllerBindingId = -1;
        _inputControlerBhv = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();

        SetButtons();

        var gameplayChoise = PlayerPrefsHelper.GetGameplayChoice();
        if (gameplayChoise == GameplayChoice.Buttons)
            Cache.SetLastEndActionClickedName(_gameplayChoiceButtons.name);
        else if (gameplayChoise == GameplayChoice.SwipesRightHanded || gameplayChoise == GameplayChoice.SwipesLeftHanded)
            Cache.SetLastEndActionClickedName(_gameplayChoiceSwipes.name);
        else if (gameplayChoise == GameplayChoice.Controller)
            Cache.SetLastEndActionClickedName(_gameplayChoiceController.name);
        GameplayButtonChoice();

        SetSensitivity(PlayerPrefsHelper.GetTouchSensitivity());

        PanelsVisuals(PlayerPrefsHelper.GetButtonsLeftPanel(), _panelLeft, isLeft: true);
        PanelsVisuals(PlayerPrefsHelper.GetButtonsRightPanel(), _panelRight, isLeft: false);
        SetOrientation(PlayerPrefsHelper.GetOrientation(), init: true);
        _keyBinding = PlayerPrefsHelper.GetKeyBinding();
        _defaultKeyBinding = PlayerPrefsHelper.GetKeyBinding(Constants.PpKeyBindingDefault);
        _controllerBinding = PlayerPrefsHelper.GetControllerBinding();
        _defaultControllerBinding = PlayerPrefsHelper.GetControllerBinding(Constants.PpControllerBindingDefault);
        for (int i = 0; i < _keyBinding.Count; ++i)
            UpdateKeyBindingVisual(i);
        for (int i = 0; i < _keyBinding.Count; ++i)
            UpdateControllerBindingVisual(i);
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
        (_gameplayChoiceController = GameObject.Find("GameplayController")).GetComponent<ButtonBhv>().EndActionDelegate = GameplayButtonChoice;
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
        _keyBindingPanelGameplay.transform.GetChild(9).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(9); };
        _keyBindingPanelGameplay.transform.GetChild(10).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(10); };
        _keyBindingPanelGameplay.transform.GetChild(11).GetComponent<ButtonBhv>().EndActionDelegate = () => { SwitchKeyBindingPanels(1); };

        _keyBindingPanelMenu.transform.GetChild(0).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(11); };
        _keyBindingPanelMenu.transform.GetChild(1).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(12); };
        _keyBindingPanelMenu.transform.GetChild(2).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(13); };
        _keyBindingPanelMenu.transform.GetChild(3).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(14); };
        _keyBindingPanelMenu.transform.GetChild(4).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(15); };
        _keyBindingPanelMenu.transform.GetChild(5).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(16); };
        _keyBindingPanelMenu.transform.GetChild(6).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(17); };
        _keyBindingPanelMenu.transform.GetChild(7).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetKeyBinding(18); };
        _keyBindingPanelMenu.transform.GetChild(8).GetComponent<ButtonBhv>().EndActionDelegate = () => { SwitchKeyBindingPanels(0); };

        _controllerPanelGameplay.transform.GetChild(0).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(0); };
        _controllerPanelGameplay.transform.GetChild(1).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(1); };
        _controllerPanelGameplay.transform.GetChild(2).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(2); };
        _controllerPanelGameplay.transform.GetChild(3).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(3); };
        _controllerPanelGameplay.transform.GetChild(4).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(4); };
        _controllerPanelGameplay.transform.GetChild(5).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(5); };
        _controllerPanelGameplay.transform.GetChild(6).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(6); };
        _controllerPanelGameplay.transform.GetChild(7).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(7); };
        _controllerPanelGameplay.transform.GetChild(8).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(8); };
        _controllerPanelGameplay.transform.GetChild(9).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(9); };
        _controllerPanelGameplay.transform.GetChild(10).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(10); };
        _controllerPanelGameplay.transform.GetChild(11).GetComponent<ButtonBhv>().EndActionDelegate = () => { SwitchControllerBindingPanels(1); };

        _controllerPanelMenu.transform.GetChild(0).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(11); };
        _controllerPanelMenu.transform.GetChild(1).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(12); };
        _controllerPanelMenu.transform.GetChild(2).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(13); };
        _controllerPanelMenu.transform.GetChild(3).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(14); };
        _controllerPanelMenu.transform.GetChild(4).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(15); };
        _controllerPanelMenu.transform.GetChild(5).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(16); };
        _controllerPanelMenu.transform.GetChild(6).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(17); };
        _controllerPanelMenu.transform.GetChild(7).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerBinding(18); };
        _controllerPanelMenu.transform.GetChild(8).GetComponent<ButtonBhv>().EndActionDelegate = () => { SwitchControllerBindingPanels(0); };

        _controllerPresetsContainer.transform.GetChild(0).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerType(ControllerType.Unknown); };
        _controllerPresetsContainer.transform.GetChild(1).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerType(ControllerType.Xbox); };
        _controllerPresetsContainer.transform.GetChild(2).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerType(ControllerType.Playstation); };
        _controllerPresetsContainer.transform.GetChild(3).GetComponent<ButtonBhv>().EndActionDelegate = () => { SetControllerType(ControllerType.Nintendo); };

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
        if (choiceButtonName.Contains("Controller"))
        {
            choiceType = GameplayChoice.Controller;
        }
        else if (choiceButtonName.Contains("Swipes"))
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
        if (gameplayChoice == GameplayChoice.Controller)
        {
            _buttonsPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _swipesPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelGameplay.GetComponent<PositionBhv>().UpdatePositions();
            _controllerPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPresetsContainer.GetComponent<PositionBhv>().UpdatePositions();
        }
#if UNITY_ANDROID
        if (gameplayChoice == GameplayChoice.Buttons)
        {
            _buttonsPanels.GetComponent<PositionBhv>().UpdatePositions();
            _swipesPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPresetsContainer.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
        }
        else if (gameplayChoice == GameplayChoice.SwipesLeftHanded || gameplayChoice == GameplayChoice.SwipesRightHanded) //Swipes
        {
            _buttonsPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _swipesPanels.GetComponent<PositionBhv>().UpdatePositions();
            _keyBindingPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPresetsContainer.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
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
            _buttonsPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _swipesPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelGameplay.GetComponent<PositionBhv>().UpdatePositions();
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPresetsContainer.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
        }
        else if (gameplayChoice == GameplayChoice.SwipesLeftHanded || gameplayChoice == GameplayChoice.SwipesRightHanded) //Mouse
        {
            _buttonsPanels.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _swipesPanels.GetComponent<PositionBhv>().UpdatePositions();
            _keyBindingPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPresetsContainer.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
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
        StartCoroutine(Helper.ExecuteAfterDelay(0.25f, () => { _listeningKeyBindingId = id; }));
        Cache.EscapeLocked = true;
        var sonicDropExplanation =  id == (int)Binding.SonicDrop ? $"\n\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}if unset, you can sonic drop\r\nby tapping{Constants.MaterialEnd} soft drop {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}twice" : "";
        _listeningPopup = Instantiator.NewPopupYesNo("Set Key", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}set new key for: {Constants.MaterialEnd}{((Binding)id).GetDescription().ToLower()}{sonicDropExplanation}", "Cancel", "Default", OnSetKey);

        void OnSetKey(bool setDefault)
        {
            if (setDefault)
            {
                CheckAlreadyBinding(_defaultKeyBinding[_listeningKeyBindingId], _listeningKeyBindingId);
                _keyBinding[_listeningKeyBindingId] = _defaultKeyBinding[_listeningKeyBindingId];
                PlayerPrefsHelper.SaveKeyBinding(_keyBinding);
                _inputControlerBhv.GetKeyBinding();
                UpdateKeyBindingVisual(_listeningKeyBindingId);
            }
            _listeningKeyBindingId = -1;
            Invoke(nameof(UnlockEscape), 0.25f);
        }
    }

    private void UnlockEscape() { Cache.EscapeLocked = false; }

    void OnGUI()
    {
        Event e = Event.current;
        if (_listeningKeyBindingId >= 0 && e.isKey && e.rawType == EventType.KeyUp)
        {
            if (_listeningKeyBindingId == 14 && e.keyCode == _keyBinding[14])
                return;
            //Debug.Log("Detected key code: " + e.keyCode);
            CheckAlreadyBinding(e.keyCode, _listeningKeyBindingId);
            _keyBinding[_listeningKeyBindingId] = e.keyCode;
            PlayerPrefsHelper.SaveKeyBinding(_keyBinding);
            _inputControlerBhv.GetKeyBinding();
            UpdateKeyBindingVisual(_listeningKeyBindingId);
            _listeningPopup.GetComponent<PopupBhv>().ExitPopup();
            _listeningKeyBindingId = -1;
            Invoke(nameof(UnlockEscape), 0.25f);
        }
    }

    private void SetControllerBinding(int id)
    {
        AutoUpdateControllerType();
        StartCoroutine(Helper.ExecuteAfterDelay(0.25f, () => { _listeningControllerBindingId = id; }));
        Cache.EscapeLocked = true;
        var sonicDropExplanation = id == (int)Binding.SonicDrop ? $"\n\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}if unset, you can sonic drop\r\nby tapping{Constants.MaterialEnd} soft drop {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}twice" : "";
        _listeningPopup = Instantiator.NewPopupYesNo("Set Input", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}set new input for: {Constants.MaterialEnd}{((Binding)id).GetDescription().ToLower()}{sonicDropExplanation}", "Cancel", "Default", OnSetInput);

        void OnSetInput(bool setDefault)
        {
            if (setDefault)
            {
                CheckAlreadyControllerBinding(_defaultControllerBinding[_listeningControllerBindingId], _listeningControllerBindingId);
                _controllerBinding[_listeningControllerBindingId] = _defaultControllerBinding[_listeningControllerBindingId];
                PlayerPrefsHelper.SaveControllerBinding(_controllerBinding);
                _inputControlerBhv.GetControllerBinding();
                UpdateControllerBindingVisual(_listeningControllerBindingId);
            }
            _listeningControllerBindingId = -1;
            Invoke(nameof(UnlockEscape), 0.25f);
        }
    }

    private void AutoUpdateControllerType()
    {
        var savedControllerType = PlayerPrefsHelper.GetControllerType();
        var joystickNames = Input.GetJoystickNames();
        var name = joystickNames.Length > 0 ? joystickNames[0] : null;
        if (Cache.ControllerName != name)
        {
            Cache.ControllerName = name;
            InvokeNextFrame(() =>
            {
                Instantiator.NewToast(name.ToLower());
            });
            if (savedControllerType == ControllerType.Unknown)
            {
                if (name.ToLower().Contains("xbox"))
                    SetControllerType(ControllerType.Xbox);
                else
                    SetControllerType(ControllerType.Unknown);
            }
        }
        else
            _controllerPresetsSelector.transform.position = _controllerPresetsContainer.transform.GetChild((int)savedControllerType).transform.position;
    }

    private void SetControllerType(ControllerType type)
    {
        PlayerPrefsHelper.SaveControllerType(type);
        _controllerPresetsSelector.transform.position = _controllerPresetsContainer.transform.GetChild((int)type).transform.position;
        for (int i = 0; i < _keyBinding.Count; ++i)
            UpdateControllerBindingVisual(i);
    }

    protected override void NormalUpdate()
    {
        if (_listeningControllerBindingId >= 0)
        {
            ++_listeningAxisFirstPass;
            JoystickInput input = null;
            foreach (var joystickButton in JoystickInput.JoystickButtons)
            {
                if (Input.GetButtonDown(joystickButton.Code))
                {
                    input = joystickButton;
                }
            }
            if (input == null)
                foreach (var joystickAxis in JoystickInput.JoystickAxes)
                {
                    if (_listeningAxisFirstPass == 1)
                    {
                        var defaultValue = Input.GetAxisRaw(joystickAxis.Code);
                        joystickAxis.DefaultValue = defaultValue;
                    }
                    else if (Input.GetAxisRaw(joystickAxis.Code) > joystickAxis.DefaultValue)
                    {
                        input = joystickAxis;
                    }
            }
            if (input != null)
            {
                Cache.InputLocked = true;
                CheckAlreadyControllerBinding(input, _listeningControllerBindingId);
                _controllerBinding[_listeningControllerBindingId] = input;
                PlayerPrefsHelper.SaveControllerBinding(_controllerBinding);
                _inputControlerBhv.GetControllerBinding();
                UpdateControllerBindingVisual(_listeningControllerBindingId);
                _listeningPopup.GetComponent<PopupBhv>().ExitPopup();
                _listeningControllerBindingId = -1;
                Invoke(nameof(UnlockEscape), 0.25f);
                Invoke(nameof(UnlockInput), 0.15f);
            }
        } else
        {
            _listeningAxisFirstPass = 0;
        }

    }

    private void UnlockInput() { Cache.InputLocked = false; }

    private void CheckAlreadyBinding(KeyCode code, int keyId)
    {
        //Check for menu controls
        if (keyId >= (int)Binding.MenuUp && keyId <= (int)Binding.Pause)
        {
            if ((keyId == (int)Binding.Pause && _keyBinding[(int)Binding.Back] == code)
                || (keyId == (int)Binding.Back && _keyBinding[(int)Binding.Pause] == code))
                return;
            for (int i = (int)Binding.MenuUp; i <= (int)Binding.Pause; ++i)
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
        for (int i = (int)Binding.HardDrop; i <= (int)Binding.SonicDrop; ++i)
        {
            if (_keyBinding[i] == code)
            {
                _keyBinding[i] = KeyCode.None;
                UpdateKeyBindingVisual(i);
            }
        }
    }

    private void CheckAlreadyControllerBinding(JoystickInput input, int keyId)
    {
        //Check for menu controls
        if (keyId >= (int)Binding.MenuUp && keyId <= (int)Binding.Pause)
        {
            if ((keyId == (int)Binding.Pause && _controllerBinding[(int)Binding.Back] == input)
                || (keyId == (int)Binding.Back && _controllerBinding[(int)Binding.Pause] == input))
                return;
            for (int i = (int)Binding.MenuUp; i <= (int)Binding.Pause; ++i)
            {
                if (_controllerBinding[i] == input)
                {
                    _controllerBinding[i] = JoystickInput.None;
                    UpdateControllerBindingVisual(i);
                }
            }
            return;
        }
        //Check for gameplay + escape
        for (int i = (int)Binding.HardDrop; i <= (int)Binding.SonicDrop; ++i)
        {
            if (_controllerBinding[i] == input)
            {
                _controllerBinding[i] = JoystickInput.None;
                UpdateControllerBindingVisual(i);
            }
        }
    }

    private void UpdateKeyBindingVisual(int id)
    {
        TMPro.TextMeshPro tmPro = null;
        if (id < 11)
            tmPro = _keyBindingPanelGameplay.transform.GetChild(id).GetComponent<TMPro.TextMeshPro>();
        else
            tmPro = _keyBindingPanelMenu.transform.GetChild(id - 11).GetComponent<TMPro.TextMeshPro>();
        var separatorId = tmPro.text.IndexOf(Constants.MaterialEnd) + Constants.MaterialEnd.Length;
        var tmpText = tmPro.text.Substring(0, separatorId);
        tmPro.text = $"{tmpText}\n{(_keyBinding[id] == KeyCode.None ? Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32) : "")}{_keyBinding[id]}";
    }

    private void UpdateControllerBindingVisual(int id)
    {
        AutoUpdateControllerType();
        TMPro.TextMeshPro tmPro = null;
        if (id < 11)
            tmPro = _controllerPanelGameplay.transform.GetChild(id).GetComponent<TMPro.TextMeshPro>();
        else
            tmPro = _controllerPanelMenu.transform.GetChild(id - 11).GetComponent<TMPro.TextMeshPro>();
        var separatorId = tmPro.text.IndexOf(Constants.MaterialEnd) + Constants.MaterialEnd.Length;
        var tmpText = tmPro.text.Substring(0, separatorId);
        tmPro.text = $"{tmpText}\n{(_controllerBinding[id] == JoystickInput.None ? Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32) : "")}{_controllerBinding[id].DisplayName(PlayerPrefsHelper.GetControllerType())}";
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
                    _menuSelector.GetComponent<MenuSelectorBhv>().MoveTo(_keyBindingPanelGameplay.transform.GetChild(11).gameObject, true);
                });
        }
        else
        {
            _keyBindingPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _keyBindingPanelMenu.GetComponent<PositionBhv>().UpdatePositions();
            if (!Cache.OnlyMouseInMenu)
                InvokeNextFrame(() =>
                {
                    _menuSelector.GetComponent<MenuSelectorBhv>().MoveTo(_keyBindingPanelMenu.transform.GetChild(8).gameObject, true);
                });
        }
    }

    private void SwitchControllerBindingPanels(int idPanel)
    {
        if (idPanel == 0)
        {
            _controllerPanelGameplay.GetComponent<PositionBhv>().UpdatePositions();
            _controllerPanelMenu.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            if (!Cache.OnlyMouseInMenu)
                InvokeNextFrame(() =>
                {
                    _menuSelector.GetComponent<MenuSelectorBhv>().MoveTo(_controllerPanelGameplay.transform.GetChild(11).gameObject, true);
                });
        }
        else
        {
            _controllerPanelGameplay.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
            _controllerPanelMenu.GetComponent<PositionBhv>().UpdatePositions();
            if (!Cache.OnlyMouseInMenu)
                InvokeNextFrame(() =>
                {
                    _menuSelector.GetComponent<MenuSelectorBhv>().MoveTo(_controllerPanelMenu.transform.GetChild(8).gameObject, true);
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
        gameplayButton.GetComponent<ButtonBhv>().LongPressActionDelegate = () => { Instantiator.NewToast(Helper.GameplayButtonToBinding(gameplayButtonName).ToLower()); };
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
            PlayerPrefsHelper.SaveControllerBinding(_defaultControllerBinding);
            _inputControlerBhv.GetControllerBinding();
            foreach (var gm in _gameplayButtons)
                Destroy(gm);
            _gameplayButtons.Clear();
            PanelButtonsVisibility(_panelLeft, _gameplayButtons);
            PanelButtonsVisibility(_panelRight, _gameplayButtons);
            Init();
        }
    }
}
