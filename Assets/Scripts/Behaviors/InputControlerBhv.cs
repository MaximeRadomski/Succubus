using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InputControlerBhv : FrameRateBehavior
{
    public MenuSelectorBhv MenuSelector;
    private Vector3 _touchPosWorld;
    private InputBhv _currentInput;
    private InputBhv _lastDownInput;
    private bool _beginPhase, _doPhase, _endPhase;
    private SceneBhv _currentScene;
    private SoundControlerBhv _soundControler;
    private GameplayControler _gameplayControler;
    private Camera _mainCamera;
    private List<KeyCode> _keyBinding;
    private List<JoystickInput> _controllerBinding;
    private List<GameObject> _availableButtons;
    private InputKeyBhv _anyInputKey;
    private bool _hasInit;
    private List<JoystickInput> _axesInUse = new List<JoystickInput>{};

    private int _currentInputLayer = -1;
    private List<GameObject> _lastSelectedGameObjects;
    private GameObject _lastSelectedGameObject;

    // THIS IS DONE FOR BETTER PERF
    //Faster than refecting on enums 
    private List<string> _inputNames;

    private void Start()
    {
        if (!_hasInit)
            Init();
    }

    private void Init()
    {
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _gameplayControler = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>();

        var menuSelectorGameObject = GameObject.Find(Constants.GoMenuSelector);
        if (menuSelectorGameObject == null)
        {
            if (_currentScene == null)
                GetScene();
            menuSelectorGameObject = _currentScene.Instantiator.NewMenuSelector();
        }
        MenuSelector = menuSelectorGameObject.GetComponent<MenuSelectorBhv>();

        GetKeyBinding();
        GetControllerBinding();
        _mainCamera = Helper.GetMainCamera();
        _inputNames = new List<string>();
        for (int i = 0; i < _keyBinding.Count; ++i)
        {
            _inputNames.Add(((Binding)i).GetDescription());
        }
        _hasInit = true;
    }

    public void GetKeyBinding()
    {
        _keyBinding = PlayerPrefsHelper.GetKeyBinding();
        if (_keyBinding[(int)Binding.SonicDrop] != KeyCode.None && _gameplayControler != null)
            _gameplayControler.SonicDropHasKey = true;
    }

    public void GetControllerBinding()
    {
        _controllerBinding = PlayerPrefsHelper.GetControllerBinding();
        if (_controllerBinding[(int)Binding.SonicDrop] != JoystickInput.None && _gameplayControler != null)
            _gameplayControler.SonicDropHasControllerInput = true;
    }

    private void GetScene()
    {
        _currentScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
    }

    protected override void FrameUpdate()
    {
        if (Cache.InputLocked)
            return;
        CheckFrameDependentGameInputs();
    }

    void OnGUI()
    {
        if (!Cache.KeyboardUp)
            return;
        if (_anyInputKey == null)
            _anyInputKey = GameObject.Find(Constants.GoKeyboard).transform.Find("InputKeyShift").GetComponent<InputKeyBhv>();
        CheckKeyBoardTextInputs();
    }

    protected override void NormalUpdate()
    {
        if (Cache.InputLocked)
            return;
        if (Cache.InputLayer == 0)
            CheckGameInputs();
        CheckMenuInputs();
        CleanAxesInUse();

        if (_currentScene == null)
            GetScene();

        // IF BACK BUTTON //
#if UNITY_ANDROID
        if (((GetBindingDown(Binding.Pause) && _currentScene is GameSceneBhv)
        || (GetBindingDown(Binding.Back) && (!(_currentScene is GameSceneBhv) || _currentScene.Paused)))
        || Input.GetKeyDown(KeyCode.Escape)
        && !Cache.EscapeLocked)
#else
        if (((GetBindingDown(Binding.Pause) && _currentScene is GameSceneBhv)
            || (GetBindingDown(Binding.Back) && (!(_currentScene is GameSceneBhv) || _currentScene.Paused)))
            && !Cache.EscapeLocked)
#endif
        {
            _soundControler.PlaySound(_soundControler.ClickIn);
        }
#if UNITY_ANDROID
        if (((GetBindingUp(Binding.Pause) && _currentScene is GameSceneBhv)
            || (GetBindingUp(Binding.Back) && (!(_currentScene is GameSceneBhv) || _currentScene.Paused)))
            || Input.GetKeyUp(KeyCode.Escape)
            && !Cache.EscapeLocked)
#else
        if (((GetBindingUp(Binding.Pause) && _currentScene is GameSceneBhv)
            || (GetBindingUp(Binding.Back) && (!(_currentScene is GameSceneBhv) || _currentScene.Paused) && (_gameplayControler == null || !_gameplayControler.GameplayOnHold)))
            && !Cache.EscapeLocked)
#endif
        {
            _soundControler.PlaySound(_soundControler.ClickOut);
            if (Cache.InputLayer > 0)
            {
                var gameObjectToDestroy = GameObject.Find(Cache.InputTopLayerNames[Cache.InputTopLayerNames.Count - 1]);
                if (gameObjectToDestroy.name.Contains("Keyboard"))
                    gameObjectToDestroy.transform.GetChild(0).GetComponent<PopupBhv>().ExitPopup();
                else
                {
                    var popupBhv = gameObjectToDestroy.GetComponent<PopupBhv>();
                    var dialogBoxBhv = gameObjectToDestroy.GetComponent<DialogBoxBhv>();
                    var toastBhv = gameObjectToDestroy.GetComponent <ToastBhv>();
                    if (popupBhv != null)
                        popupBhv.ExitPopup();
                    else if (dialogBoxBhv != null)
                        dialogBoxBhv.PrevSentence();
                    else if (toastBhv != null)
                        toastBhv.ExitToast();
                    else if (!_currentScene.Paused)
                        _mainCamera.gameObject.GetComponent<CameraBhv>().Unfocus();
                }
                //Cache.DecreaseInputLayer();
                //Destroy(gameObjectToDestroy);
            }
            else
            {
                if (!_currentScene.Paused && _currentScene.CanGoPreviousScene)
                    _currentScene.PauseOrPrevious();
                else
                    _currentScene.Resume();
            }
            return;
        }
        var currentFrameInputLayer = Cache.InputLayer;
        // IF SCREEN TOUCH //
        if (Input.touchCount > 0)
        {
            if (!Cache.OnlyMouseInMenu)
            {
                Cache.OnlyMouseInMenu = true;
                ResetMenuSelector();
            }
            for (int i = 0; i < Input.touchCount; i++)
            {
                _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.GetTouch(i).position);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, _mainCamera.transform.forward);
                foreach (var hitInformation in hitsInformation)
                {
                    if (hitInformation.collider != null)
                    {
                        CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                        _currentInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
                        if (_currentInput == null
                            || _currentInput.Layer < currentFrameInputLayer
                            || IsTheLowestInput(_currentInput, hitsInformation)
                            || IsUnderSprite(_currentInput, hitsInformation)
                            || (_currentInput.gameObject.GetComponent<MaskLinkerBhv>() != null && !Helper.IsSpriteRendererVisible(_currentInput.gameObject, _currentInput.gameObject.GetComponent<MaskLinkerBhv>().Mask)))
                            continue;
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            _lastDownInput = _currentInput;
                            _currentInput.BeginAction(touchPosWorld2D);
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended && _lastDownInput?.name == _currentInput.name)
                        {
                            Cache.SetLastEndActionClickedName(_currentInput.name);
                            _currentInput.EndAction(touchPosWorld2D);
                            _currentInput = null;
                            _lastDownInput = null;
                        }
                        else
                            _currentInput.DoAction(touchPosWorld2D);
                    }
                    else
                        CancelCurrentObjectIfNewBeforeEnd();
                }
            }
        }
        else
        {
            _beginPhase = _doPhase = _endPhase = false;
            // IF MOUSE //
            if ((_beginPhase = Input.GetMouseButtonDown(0))
                || (_endPhase = Input.GetMouseButtonUp(0))
                || (_doPhase = Input.GetMouseButton(0)))
            {
                if (!Cache.OnlyMouseInMenu)
                {
                    Cache.OnlyMouseInMenu = true;
                    ResetMenuSelector();
                }
                _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, _mainCamera.transform.forward);
                foreach (var hitInformation in hitsInformation)
                {
                    if (hitInformation.collider != null)
                    {
                        CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                        _currentInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
                        if (_currentInput == null
                            || _currentInput.Layer < currentFrameInputLayer
                            || IsTheLowestInput(_currentInput, hitsInformation)
                            || IsUnderSprite(_currentInput, hitsInformation)
                            || (_currentInput.gameObject.GetComponent<MaskLinkerBhv>() != null && !Helper.IsSpriteRendererVisible(_currentInput.gameObject, _currentInput.gameObject.GetComponent<MaskLinkerBhv>().Mask)))
                            continue;
                        if (_beginPhase)
                        {
                            _lastDownInput = _currentInput;
                            _currentInput.BeginAction(touchPosWorld2D);
                        }
                        else if (_endPhase && _lastDownInput?.name == _currentInput.name)
                        {
                            Cache.SetLastEndActionClickedName(_currentInput.name);
                            _currentInput.EndAction(touchPosWorld2D);
                            _currentInput = null;
                            _lastDownInput = null;
                        }
                        else if (_doPhase)
                            _currentInput.DoAction(touchPosWorld2D);
                    }
                    else
                        CancelCurrentObjectIfNewBeforeEnd();
                }
            }
            // ELSE //
            else
                _touchPosWorld = new Vector3(-99, -99, -99);
        }
    }

    private void CancelCurrentObjectIfNewBeforeEnd(GameObject touchedGameObject = null)
    {
        if (_currentInput == null || _currentInput.gameObject == touchedGameObject ||
            (_currentInput is ButtonBhv && ((ButtonBhv)_currentInput).LongPressActionDelegate != null))
            return;

        _currentInput.CancelAction();
        _currentInput = null;
        //_lastDownInput = null; Not Sure
    }

    private bool IsTheLowestInput(InputBhv currentInput, RaycastHit2D[] hitsInformation)
    {
        int highest = -1;
        foreach (var hitInformation in hitsInformation)
        {
            var tmpInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
            if (tmpInput != null && tmpInput.Layer > highest)
                highest = tmpInput.Layer;
        }
        return currentInput.Layer < highest;
    }

    private bool IsUnderSprite(InputBhv currentInput, RaycastHit2D[] hitsInformation)
    {
        var currentSprite = currentInput.GetComponent<SpriteRenderer>();
        if (currentSprite == null)
            return false;
        foreach (var hitInformation in hitsInformation)
        {
            var tmpSprite = hitInformation.transform.gameObject.GetComponent<SpriteRenderer>();
            if (tmpSprite != null)
            {
                var tmpLayerId = SortingLayer.GetLayerValueFromName(tmpSprite.sortingLayerName) + 1000;
                var currentLayerId = SortingLayer.GetLayerValueFromName(currentSprite.sortingLayerName) + 1000;
                if (tmpLayerId > currentLayerId)
                    return true;
            }
        }
        return false;
    }

    //0: Up
    //1: Down
    //2: Left
    //3: Right
    //4: Clock
    //5: Anti-Clock
    //6: Hold
    //7: Item
    //8: Special
    //9: Back-Pause

    private void CheckFrameDependentGameInputs()
    {
        if (_gameplayControler == null || _gameplayControler.CurrentPiece == null)
            return;
        if (GetBinding(Binding.Left))
        {
            _gameplayControler.LeftHeld();
        }
        if (GetBinding(Binding.Right))
        {
            _gameplayControler.RightHeld();
        }
        HandleFrameKeysPressOrHeld();
    }

    public bool GetBindingDown(Binding binding)
    {
        var key = _keyBinding[(int)binding];
        var joystickInput = _controllerBinding[(int)binding];
        bool cameThroughAxis = false;
        if (Input.GetKeyDown(key)
            || (joystickInput.Type == JoystickInputType.button && Input.GetButtonDown(joystickInput.Code))
            || (joystickInput.Type == JoystickInputType.axis && (cameThroughAxis = Input.GetAxisRaw(joystickInput.Code) > joystickInput.DefaultValue && !_axesInUse.Contains(joystickInput))))
        {
            if (cameThroughAxis)
                _axesInUse.Add(joystickInput);
            return true;
        }
        return false;
    }

    public bool GetBinding(Binding binding)
    {
        var key = _keyBinding[(int)binding];
        var joystickInput = _controllerBinding[(int)binding];
        bool cameThroughAxis = false;
        if (Input.GetKey(key)
            || (joystickInput.Type == JoystickInputType.button && Input.GetButton(joystickInput.Code))
            || (joystickInput.Type == JoystickInputType.axis && (cameThroughAxis = Input.GetAxisRaw(joystickInput.Code) > joystickInput.DefaultValue && _axesInUse.Contains(joystickInput))))
        {
            return true;
        }
        return false;
    }

    public bool GetBindingUp(Binding binding)
    {
        var key = _keyBinding[(int)binding];
        var joystickInput = _controllerBinding[(int)binding];
        bool cameThroughAxis = false;
        if (Input.GetKeyUp(key)
            || (joystickInput.Type == JoystickInputType.button && Input.GetButtonUp(joystickInput.Code))
            || (joystickInput.Type == JoystickInputType.axis && (cameThroughAxis = Input.GetAxisRaw(joystickInput.Code) == joystickInput.DefaultValue && _axesInUse.Contains(joystickInput))))
        {
            if (cameThroughAxis)
                _axesInUse.Remove(joystickInput);
            return true;
        }
        return false;
    }

    private void CheckGameInputs()
    {
        if (_gameplayControler == null || _gameplayControler.CurrentPiece == null)
            return;
        if (GetBindingDown(Binding.HardDrop))
        {
            _gameplayControler.HardDrop();
        }
        if (GetBindingDown(Binding.SoftDrop))
        {
            _gameplayControler.Down();
        }
        if (GetBinding(Binding.SoftDrop))
        {
            _gameplayControler.SoftDropHeld();
        }
        if (GetBindingUp(Binding.SoftDrop))
        {
            _gameplayControler.DownReleased();
        }
        if (GetBindingDown(Binding.Left))
        {
            _gameplayControler.Left();
        }
        if (GetBindingUp(Binding.Left) || GetBindingUp(Binding.Right))
        {
            _gameplayControler.DirectionReleased();
        }
        if (GetBindingDown(Binding.Right))
        {
            _gameplayControler.Right();
        }
        if (GetBindingDown(Binding.Clock))
        {
            _gameplayControler.Clock();
        }
        if (GetBindingDown(Binding.AntiClock))
        {
            _gameplayControler.AntiClock();
        }
        if (GetBindingDown(Binding.Hold))
        {
            _gameplayControler.Hold();
        }
        if (GetBindingDown(Binding.Item))
        {
            _gameplayControler.Item();
        }
        if (GetBindingDown(Binding.Special))
        {
            _gameplayControler.Special();
        }
        if (GetBindingDown(Binding.Rotation180))
        {
            _gameplayControler.Rotation180();
        }
        if (GetBindingDown(Binding.SonicDrop))
        {
            _gameplayControler.SonicDrop();
        }
        if (GetBindingDown(Binding.Restart))
        {
            if (_currentScene == null)
                GetScene();
            if (_currentScene is TrainingFreeGameSceneBhv trainingScene && Cache.InputLayer == 0)
                trainingScene.AskRestartTraining();
        }
        HandleFrameKeysPressOrHeld();
    }

    private void CleanAxesInUse()
    {
        if (_axesInUse.Count > 0)
            for (int i = _axesInUse.Count - 1; i >= 0; --i)
            {
                if (Input.GetAxisRaw(_axesInUse[i].Code) == _axesInUse[i].DefaultValue)
                    _axesInUse.RemoveAt(i);
            }
    }

    private void HandleFrameKeysPressOrHeld()
    {
        if (Input.anyKeyDown || Input.anyKey || AnyJoyStickButton())
        {
            for (int i = 0; i < Constants.BindingsCount; ++i)
            {
                if (GetBindingDown((Binding)i) || GetBinding((Binding)i))
                {
                    _gameplayControler.AddFrameKeyPressOrHeld(_inputNames[i]);
                }
            }
        }
        _gameplayControler.UpdateFrameKeysPressOrHeld();
    }

    private bool AnyJoyStickButton()
    {
        foreach (var joystickButton in JoystickInput.JoystickButtons)
        {
            if (Input.GetButtonDown(joystickButton.Code))
            {
                return true;
            }
        }
        foreach (var joystickAxis in JoystickInput.JoystickAxes)
        {
            if (Input.GetAxisRaw(joystickAxis.Code) > joystickAxis.DefaultValue)
            {
                return true;
            }
        }
        return false;
    }

    public void InitMenuKeyboardInputs(Vector3? preferedResetPos = null)
    {
        if (!_hasInit)
            Init();
        var buttonTaggedGameObjects = GameObject.FindGameObjectsWithTag(Constants.TagButton);
        _availableButtons = new List<GameObject>();
        if (buttonTaggedGameObjects.Length > 0)
        {
            for (int i = buttonTaggedGameObjects.Length - 1; i >= 0; i--)
            {
                var buttonBhv = buttonTaggedGameObjects[i].GetComponent<ButtonBhv>();
                if (buttonBhv != null
                    && (buttonBhv.BeginActionDelegate != null || buttonBhv.DoActionDelegate != null || buttonBhv.EndActionDelegate != null)
                    && buttonBhv.Layer == Cache.InputLayer
                    && buttonBhv.GetComponent<BoxCollider2D>().enabled == true)
                {
                    if (_gameplayControler != null && buttonBhv.Layer == 0)
                        continue;
                    _availableButtons.Add(buttonTaggedGameObjects[i]);
                }
            }
        }
        _currentScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
        if (Cache.InputLayer > _currentInputLayer)
        {
            if (_lastSelectedGameObjects == null)
                _lastSelectedGameObjects = new List<GameObject>();
            if (_lastSelectedGameObject != null)
                _lastSelectedGameObjects.Add(_lastSelectedGameObject);
            ResetMenuSelector(preferedResetPos);
        }
        else if (Cache.InputLayer < _currentInputLayer)
        {
            if (_lastSelectedGameObjects != null && _lastSelectedGameObjects.Count > 0)
            {
                _lastSelectedGameObject = _lastSelectedGameObjects[_lastSelectedGameObjects.Count - 1];
                if (_lastSelectedGameObject != null)
                {
                    if (_mainCamera == null)
                        _mainCamera = Helper.GetMainCamera();
                    if (Helper.IsInsideCamera(_mainCamera, _lastSelectedGameObject.transform.position))
                        MenuSelector.MoveTo(_lastSelectedGameObject);
                    else
                        ResetMenuSelector(preferedResetPos);
                }
                else
                    ResetMenuSelector(preferedResetPos);
                _lastSelectedGameObjects.RemoveAt(_lastSelectedGameObjects.Count - 1);
            }
            else
                ResetMenuSelector(preferedResetPos);
        }
        else
        {
            _lastSelectedGameObject = null;
            ResetMenuSelector(preferedResetPos);
        }
        _currentInputLayer = Cache.InputLayer;
    }

    public void ResetMenuSelector(Vector3? preferedResetPos = null)
    {
        if (!_hasInit)
            Init();
        if (_currentScene == null)
            GetScene();
        MenuSelector?.Reset(Cache.OnlyMouseInMenu ? null : preferedResetPos);
        if (!Cache.OnlyMouseInMenu && (_availableButtons != null && _availableButtons.Count > 0))
        {
            //if (_mainCamera != null && _mainCamera.GetComponent<CameraBhv>().IsSliding)
            //    return;
            FindNearest(Direction.Down, soundMuted: true, reset: true);
        }
    }

    private void CheckMenuInputs()
    {
        if (MenuSelector == null || Cache.KeyboardUp)
            return;
        if (_gameplayControler != null && _gameplayControler.SceneBhv != null && !_gameplayControler.SceneBhv.Paused && !_gameplayControler.GameplayOnHold)
        {
            MenuSelector.Reset();
            _currentInputLayer = 0;
            return;
        }
        if (_currentInputLayer != Cache.InputLayer || _availableButtons == null || _availableButtons.Count == 0)
        {
            if (!Cache.OnlyMouseInMenu)
                InitMenuKeyboardInputs();
            else if (_lastSelectedGameObjects != null)
                _lastSelectedGameObjects.Clear();
        }
        if (GetBindingDown(Binding.MenuUp))
        {
            FindNearest(Direction.Up);
        }
        else if (GetBindingDown(Binding.MenuDown))
        {
            FindNearest(Direction.Down);
        }
        else if (GetBindingDown(Binding.MenuLeft))
        {
            FindNearest(Direction.Left);
        }
        else if (GetBindingDown(Binding.MenuRight))
        {
            FindNearest(Direction.Right);
        }
        else if (GetBindingDown(Binding.MenuSelect))
        {
            ButtonOnSelector(Direction.Down);
        }
        else if (GetBindingUp(Binding.MenuSelect))
        {
            ButtonOnSelector(Direction.Up);
        }
        else if (GetBinding(Binding.MenuSelect))
        {
            ButtonOnSelector(Direction.None);
        }
    }

    private void CheckKeyBoardTextInputs()
    {
        Event e = Event.current;
        if (e.isKey && e.rawType == EventType.KeyDown && e.keyCode != KeyCode.None)
        {
            if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace || e.keyCode == KeyCode.Clear)
                _anyInputKey.Del();
            else if (e.keyCode == KeyCode.CapsLock)
                _anyInputKey.Shift();
            else if (e.keyCode == KeyCode.LeftShift || e.keyCode == KeyCode.RightShift)
                _anyInputKey.Shift(isUpper: true);
            else if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter || e.keyCode == KeyCode.Tab)
                _anyInputKey.Validate();
            else if (KeyCodeService.ForbiddenTextKeys.Contains(e.keyCode))
                return;
            else
            {
                _anyInputKey.AddLetter(KeyCodeService.KeyCodeToString(e.keyCode));
            }                
        }
        else if (e.isKey && e.rawType == EventType.KeyUp && e.keyCode != KeyCode.None)
        {
            if (e.keyCode == KeyCode.LeftShift || e.keyCode == KeyCode.RightShift)
                _anyInputKey.Shift(isUpper: false);
        }
    }

    private void FindNearest(Direction direction, float? visionConeMult = null, bool retry = false, bool soundMuted = false, bool reset = false)
    {
        if (Cache.OnlyMouseInMenu)
            direction = Direction.Down;
        Cache.OnlyMouseInMenu = false;
        var minDistance = 99.0f;
        GameObject selectedGameObject = null;
        if (_mainCamera == null)
        {
            _mainCamera = Helper.GetMainCamera();
            if (_mainCamera == null)
                return;
        }

        if (_availableButtons == null)
            return;
        else if (_availableButtons.Count > 1)
        {
            var hasMenuSelectorResetButton = false;
            foreach (var button in _availableButtons)
            {
                if (button == null)
                    continue;
                var precision = 1.0f;
                var lastConeMult = Constants.BaseButtonVisionConeMult;
                if (_lastSelectedGameObject != null)
                    lastConeMult = _lastSelectedGameObject.GetComponent<ButtonBhv>().ConeVisionMult;
                visionConeMult = visionConeMult == null ? lastConeMult : visionConeMult;

                if (button == null
                    || (direction == Direction.Up && button.transform.position.y < MenuSelector.transform.position.y + precision / 2)
                    || (direction == Direction.Down && button.transform.position.y > MenuSelector.transform.position.y - precision / 2)
                    || (direction == Direction.Left && button.transform.position.x > MenuSelector.transform.position.x - precision / 2)
                    || (direction == Direction.Right && button.transform.position.x < MenuSelector.transform.position.x + precision / 2)
                    || button.GetComponent<ButtonBhv>().Layer != Cache.InputLayer
                    || (!soundMuted && !Helper.IsInsideCamera(_mainCamera, button.transform.position))
                    || (!soundMuted && button.GetComponent<MaskLinkerBhv>() != null && !Helper.IsSpriteRendererVisible(button, button.GetComponent<MaskLinkerBhv>().Mask)))
                    continue;
                if (reset && button.GetComponent<ButtonBhv>().IsMenuSelectorResetButton && button.GetComponent<ButtonBhv>().Layer == Cache.InputLayer)
                {
                    hasMenuSelectorResetButton = true;
                    selectedGameObject = button;
                }
                var distance = Vector2.Distance(button.transform.position, MenuSelector.transform.position);
                if (hasMenuSelectorResetButton == false && distance < minDistance && distance > precision && IsInsideVisionCone(MenuSelector.transform.position, button.transform.position, direction, visionConeMult.Value))
                {                        
                    minDistance = distance;
                    selectedGameObject = button;
                }
            }
        }
        else if (_availableButtons.Count == 1)
        {
            selectedGameObject = _availableButtons[0];
        }

        visionConeMult -= 0.49f;

        if (selectedGameObject != null)
        {
            _lastSelectedGameObject = selectedGameObject;
            MenuSelector.MoveTo(selectedGameObject, soundMuted);
        }
        else if (visionConeMult != null && visionConeMult > 0)
        {
            FindNearest(direction, visionConeMult, retry);
        }
        else if ((visionConeMult == null || visionConeMult <= 0.0f) && retry == false)
        {
            var resetSize = 2.0f;
            if (direction == Direction.Up)
                MenuSelector.Reset(new Vector3(MenuSelector.transform.position.x, -_mainCamera.orthographicSize - resetSize, 0.0f) + _mainCamera.transform.position);
            else if (direction == Direction.Down)
                MenuSelector.Reset(new Vector3(MenuSelector.transform.position.x, _mainCamera.orthographicSize + resetSize, 0.0f) + _mainCamera.transform.position);
            else if (direction == Direction.Left)
                MenuSelector.Reset(new Vector3((_mainCamera.orthographicSize * _mainCamera.aspect) + resetSize, MenuSelector.transform.position.y, 0.0f) + _mainCamera.transform.position);
            else if (direction == Direction.Right)
                MenuSelector.Reset(new Vector3((-_mainCamera.orthographicSize * _mainCamera.aspect) - resetSize, MenuSelector.transform.position.y, 0.0f) + _mainCamera.transform.position);
            FindNearest(direction, 0.50f, true);
        }
    }

    private bool IsInsideVisionCone(Vector2 watcher, Vector2 target, Direction direction, float coneMult)
    {
        var horDistance = Mathf.Abs(watcher.x - target.x);
        var verDistance = Mathf.Abs(watcher.y - target.y);
        if (direction == Direction.Left || direction == Direction.Right) 
            return verDistance <= (horDistance / coneMult) && horDistance >= (verDistance * coneMult);
        else if (direction == Direction.Up || direction == Direction.Down)
            return horDistance <= (verDistance / coneMult) && verDistance >= (horDistance * coneMult);
        return false;
    }

    private void ButtonOnSelector(Direction direction)
    {
        if (Cache.OnlyMouseInMenu || _availableButtons == null || _availableButtons.Count == 0)
        {
            if (Cache.OnlyMouseInMenu)
                FindNearest(Direction.Down);
            return;
        }
        var minDistance = 99.0f;
        GameObject selectedGameObject = null;

        foreach (var button in _availableButtons)
        {
            if (button == null)
                continue;
            var boxCollider = button.GetComponent<BoxCollider2D>();
            var offset = new Vector3(boxCollider.offset.x, boxCollider.offset.y, 0.0f);
            var currentDistance = Vector2.Distance(button.transform.position + offset, MenuSelector.transform.position);
            if (currentDistance < minDistance && currentDistance < 1.0f)
            {
                minDistance = currentDistance;
                selectedGameObject = button;
            }
        }

        if (selectedGameObject != null)
        {
            var buttonBhv = selectedGameObject.GetComponent<ButtonBhv>();
            Cache.LastEndActionClickedName = selectedGameObject.name;
            if (direction == Direction.Down)
                buttonBhv.BeginAction(selectedGameObject.transform.position);
            else if (direction == Direction.None)
                buttonBhv.DoAction(selectedGameObject.transform.position);
            else
            {
                buttonBhv.EndAction(selectedGameObject.transform.position);
                MenuSelector.Click(selectedGameObject);
            }
        }
    }
}
