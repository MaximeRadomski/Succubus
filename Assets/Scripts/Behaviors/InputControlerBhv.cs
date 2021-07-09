using System.Collections;
using System.Collections.Generic;
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
    private List<GameObject> _availableButtons;
    private InputKeyBhv _anyInputKey;
    private bool _hasInit;

    private int _currentInputLayer = -1;
    private List<GameObject> _lastSelectedGameObjects;
    private GameObject _lastSelectedGameObject;
    private float _lastDownSoftDrop = -1;

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
#if !UNITY_ANDROID
        var menuSelectorGameObject = GameObject.Find(Constants.GoMenuSelector);
        if (menuSelectorGameObject == null)
        {
            if (_currentScene == null)
                GetScene();
            menuSelectorGameObject = _currentScene.Instantiator.NewMenuSelector();
        }
        MenuSelector = menuSelectorGameObject.GetComponent<MenuSelectorBhv>();
#endif
        GetKeyBinding();
        _mainCamera = Helper.GetMainCamera();
        _inputNames = new List<string>();
        for (int i = 0; i < _keyBinding.Count; ++i)
        {
            _inputNames.Add(((KeyBinding)i).GetDescription());
        }
        _hasInit = true;
    }

    public void GetKeyBinding()
    {
        _keyBinding = PlayerPrefsHelper.GetKeyBinding();
    }

    private void GetScene()
    {
        _currentScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
    }

    protected override void FrameUpdate()
    {
        if (Constants.InputLocked)
            return;
#if !UNITY_ANDROID || UNITY_EDITOR
        CheckFrameDependentGameKeyboardInputs();
#endif
    }

    void OnGUI()
    {
        if (!Constants.KeyboardUp)
            return;
        if (_anyInputKey == null)
            _anyInputKey = GameObject.Find(Constants.GoKeyboard).transform.Find("InputKeyShift").GetComponent<InputKeyBhv>();
        CheckKeyBoardTextInputs();
    }

    protected override void NormalUpdate()
    {
        if (Constants.InputLocked)
            return;
#if !UNITY_ANDROID || UNITY_EDITOR
        CheckGameKeyboardInputs();
        CheckMenuKeyboardInputs();
#endif
        // IF BACK BUTTON //
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape) && !Constants.EscapeLocked)
#else
        if (Input.GetKeyDown(_keyBinding[15]) && !Constants.EscapeLocked)
#endif
        {
            _soundControler.PlaySound(_soundControler.ClickIn);
        }
#if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape) && !Constants.EscapeLocked)
#else
        if (Input.GetKeyUp(_keyBinding[15]) && !Constants.EscapeLocked)
#endif
        {
            _soundControler.PlaySound(_soundControler.ClickOut);
            if (_currentScene == null)
                GetScene();
            if (Constants.InputLayer > 0)
            {
                var gameObjectToDestroy = GameObject.Find(Constants.InputTopLayerNames[Constants.InputTopLayerNames.Count - 1]);
                if (gameObjectToDestroy.name.Contains("Keyboard"))
                    gameObjectToDestroy.transform.GetChild(0).GetComponent<PopupBhv>().ExitPopup();
                else
                {
                    var popupBhv = gameObjectToDestroy.GetComponent<PopupBhv>();
                    var dialogBoxBhv = gameObjectToDestroy.GetComponent<DialogBoxBhv>();
                    if (popupBhv != null)
                        popupBhv.ExitPopup();
                    else if (dialogBoxBhv != null)
                        dialogBoxBhv.PrevSentence();
                    else if (!_currentScene.Paused)
                        _mainCamera.gameObject.GetComponent<CameraBhv>().Unfocus();
                }
                //Constants.DecreaseInputLayer();
                //Destroy(gameObjectToDestroy);
            }
            else
            {
                if (!_currentScene.Paused)
                    _currentScene.PauseOrPrevious();
                else
                    _currentScene.Resume();
            }
            return;
        }
        var currentFrameInputLayer = Constants.InputLayer;
        // IF SCREEN TOUCH //
        if (Input.touchCount > 0)
        {
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
                            Constants.SetLastEndActionClickedName(_currentInput.name);
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
#if !UNITY_ANDROID
                if (!Constants.OnlyMouseInMenu)
                {
                    Constants.OnlyMouseInMenu = true;
                    ResetMenuSelector();
                }
#endif
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
                            Constants.SetLastEndActionClickedName(_currentInput.name);
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
        if (_currentInput == null || _currentInput.gameObject == touchedGameObject)
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
            if (tmpSprite != null &&
                SortingLayer.GetLayerValueFromName(tmpSprite.sortingLayerName) > SortingLayer.GetLayerValueFromName(currentSprite.sortingLayerName))
                return true;
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

    private void CheckFrameDependentGameKeyboardInputs()
    {
        if (_gameplayControler == null || _gameplayControler.CurrentPiece == null)
            return;
        if (Input.GetKey(_keyBinding[2]))
        {
            _gameplayControler.LeftHeld();
        }
        if (Input.GetKey(_keyBinding[3]))
        {
            _gameplayControler.RightHeld();
        }
        HandleFrameKeysPressOrHeld();
    }

    private void CheckGameKeyboardInputs()
    {
        if (_gameplayControler == null || _gameplayControler.CurrentPiece == null)
            return;
        if (Input.GetKeyDown(_keyBinding[0]))
        {
            _gameplayControler.HardDrop();
        }
        if (Input.GetKeyDown(_keyBinding[1]))
        {
            _gameplayControler.Down();
        }
        if (Input.GetKeyDown(_keyBinding[1]))
        {
            if (_lastDownSoftDrop >= Time.time - 0.2f)
            {
                _gameplayControler.SoftDropStomp();
            }
            _lastDownSoftDrop = Time.time;
        }
        if (Input.GetKey(_keyBinding[1]))
        {
            _gameplayControler.SoftDropHeld();
        }
        if (Input.GetKeyDown(_keyBinding[2]))
        {
            _gameplayControler.Left();
        }
        if (Input.GetKeyUp(_keyBinding[2]) || Input.GetKeyUp(_keyBinding[3]))
        {
            _gameplayControler.DirectionReleased();
        }
        if (Input.GetKeyDown(_keyBinding[3]))
        {
            _gameplayControler.Right();
        }
        if (Input.GetKeyDown(_keyBinding[4]))
        {
            _gameplayControler.Clock();
        }
        if (Input.GetKeyDown(_keyBinding[5]))
        {
            _gameplayControler.AntiClock();
        }
        if (Input.GetKeyDown(_keyBinding[6]))
        {
            _gameplayControler.Hold();
        }
        if (Input.GetKeyDown(_keyBinding[7]))
        {
            _gameplayControler.Item();
        }
        if (Input.GetKeyDown(_keyBinding[8]))
        {
            _gameplayControler.Special();
        }
        if (Input.GetKeyDown(_keyBinding[9]))
        {
            _gameplayControler.Rotation180();
        }
        if (Input.GetKeyDown(_keyBinding[16]))
        {
            if (_currentScene == null)
                GetScene();
            if (_currentScene is TrainingFreeGameSceneBhv trainingScene && Constants.InputLayer == 0)
                trainingScene.AskRestartTraining();
        }
        HandleFrameKeysPressOrHeld();
    }

    private void HandleFrameKeysPressOrHeld()
    {
        if (Input.anyKeyDown || Input.anyKey)
        {
            for (int i = 0; i < _keyBinding.Count; ++i)
            {
                if (Input.GetKeyDown(_keyBinding[i]) || Input.GetKey(_keyBinding[i]))
                {
                    _gameplayControler.AddFrameKeyPressOrHeld(_inputNames[i]);
                }
            }
        }
        _gameplayControler.UpdateFrameKeysPressOrHeld();
    }

    public void InitMenuKeyboardInputs(Vector3? preferedResetPos = null)
    {
#if UNITY_ANDROID
        return;
#else
        if (!_hasInit)
            Init();
        var allGameObjects = FindObjectsOfType<GameObject>();
        _availableButtons = new List<GameObject>();
        if (allGameObjects.Length > 0)
        {
            for (int i = allGameObjects.Length - 1; i >= 0; i--)
            {
                var buttonBhv = allGameObjects[i].GetComponent<ButtonBhv>();
                if (buttonBhv != null
                    && (buttonBhv.BeginActionDelegate != null || buttonBhv.DoActionDelegate != null || buttonBhv.EndActionDelegate != null)
                    && buttonBhv.Layer == Constants.InputLayer
                    && buttonBhv.GetComponent<BoxCollider2D>().enabled == true)
                {
                    if (_gameplayControler != null && buttonBhv.Layer == 0)
                        continue;
                    _availableButtons.Add(allGameObjects[i]);
                }
            }
        }
        _currentScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
        if (Constants.InputLayer > _currentInputLayer)
        {
            if (_lastSelectedGameObjects == null)
                _lastSelectedGameObjects = new List<GameObject>();
            if (_lastSelectedGameObject != null)
                _lastSelectedGameObjects.Add(_lastSelectedGameObject);
            ResetMenuSelector(preferedResetPos);
        }
        else if (Constants.InputLayer < _currentInputLayer)
        {
            if (_lastSelectedGameObjects != null && _lastSelectedGameObjects.Count > 0)
            {
                _lastSelectedGameObject = _lastSelectedGameObjects[_lastSelectedGameObjects.Count - 1];
                if (_mainCamera == null)
                    _mainCamera = Helper.GetMainCamera();
                if (Helper.IsInsideCamera(_mainCamera, _lastSelectedGameObject.transform.position))
                    MenuSelector.MoveTo(_lastSelectedGameObject);
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
        _currentInputLayer = Constants.InputLayer;
#endif
    }

    public void ResetMenuSelector(Vector3? preferedResetPos = null)
    {
        if (!_hasInit)
            Init();
        if (_currentScene == null)
            GetScene();
        MenuSelector?.Reset(Constants.OnlyMouseInMenu ? null : preferedResetPos);
        if (!Constants.OnlyMouseInMenu && (_availableButtons != null && _availableButtons.Count > 0))
        {
            //if (_mainCamera != null && _mainCamera.GetComponent<CameraBhv>().IsSliding)
            //    return;
            FindNearest(Direction.Down, soundMuted: true, reset: true);
        }
    }

    private void CheckMenuKeyboardInputs()
    {
        if (MenuSelector == null || Constants.KeyboardUp)
            return;
        if (_gameplayControler != null && _gameplayControler.SceneBhv != null && !_gameplayControler.SceneBhv.Paused)
        {
            MenuSelector.Reset();
            _currentInputLayer = 0;
            return;
        }
        if (_currentInputLayer != Constants.InputLayer || _availableButtons == null || _availableButtons.Count == 0)
        {
            if (!Constants.OnlyMouseInMenu)
                InitMenuKeyboardInputs();
            else if (_lastSelectedGameObjects != null)
                _lastSelectedGameObjects.Clear();
        }
        if (Input.GetKeyDown(_keyBinding[10]))
            FindNearest(Direction.Up);
        else if (Input.GetKeyDown(_keyBinding[11]))
            FindNearest(Direction.Down);
        else if (Input.GetKeyDown(_keyBinding[12]))
            FindNearest(Direction.Left);
        else if (Input.GetKeyDown(_keyBinding[13]))
            FindNearest(Direction.Right);
        else if (Input.GetKeyDown(_keyBinding[14]))
            ButtonOnSelector();
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
        if (Constants.OnlyMouseInMenu)
            direction = Direction.Down;
        Constants.OnlyMouseInMenu = false;
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
                    || button.GetComponent<ButtonBhv>().Layer != Constants.InputLayer
                    || (!soundMuted && !Helper.IsInsideCamera(_mainCamera, button.transform.position))
                    || (!soundMuted && button.GetComponent<MaskLinkerBhv>() != null && !Helper.IsSpriteRendererVisible(button, button.GetComponent<MaskLinkerBhv>().Mask)))
                    continue;
                if (reset && button.GetComponent<ButtonBhv>().IsMenuSelectorResetButton && button.GetComponent<ButtonBhv>().Layer == Constants.InputLayer)
                {
                    hasMenuSelectorResetButton = true;
                    selectedGameObject = button;
                }
                var distance = Vector2.Distance(button.transform.position, MenuSelector.transform.position);
                if (hasMenuSelectorResetButton == false && distance < minDistance && distance > precision && IsInsideVisionCone(MenuSelector.transform.position, button.transform.position, direction, visionConeMult.Value)
                    && (_lastSelectedGameObject == null || button.name != _lastSelectedGameObject.name))
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

    private void ButtonOnSelector()
    {
        if (Constants.OnlyMouseInMenu || _availableButtons == null || _availableButtons.Count == 0)
        {
            if (Constants.OnlyMouseInMenu)
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
            MenuSelector.Click(selectedGameObject);
            Constants.LastEndActionClickedName = selectedGameObject.name;
            var buttonBhv = selectedGameObject.GetComponent<ButtonBhv>();
            buttonBhv.BeginAction(selectedGameObject.transform.position);
            buttonBhv.DoAction(selectedGameObject.transform.position);
            buttonBhv.EndAction(selectedGameObject.transform.position);
            
        }
    }
}
