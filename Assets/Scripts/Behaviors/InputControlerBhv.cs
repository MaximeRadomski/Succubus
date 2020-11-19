using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControlerBhv : MonoBehaviour
{
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

    private MenuSelectorBhv _menuSelector;
    private int _currentInputLayer = -1;
    private List<GameObject> _lastPositions;

    // THIS IS DONE FOR BETTER PERF
    //Faster than refecting on enums 
    private List<string> _inputNames;

    private void Start()
    {
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _gameplayControler = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>();
        _menuSelector = GameObject.Find(Constants.GoMenuSelector).GetComponent<MenuSelectorBhv>();
        _keyBinding = PlayerPrefsHelper.GetKeyBinding();
        _mainCamera = Helper.GetMainCamera();
        _inputNames = new List<string>();
        for (int i = 0; i < _keyBinding.Count; ++i)
        {
            _inputNames.Add(((KeyBinding)i).GetDescription());
        }
    }

    void Update()
    {
        if (Constants.InputLocked)
            return;
#if !UNITY_ANDROID
        CheckGameKeyboardInputs();
        CheckMenuKeyboardInputs();
#endif
        // IF BACK BUTTON //
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(_keyBinding[9])) && !Constants.EscapeLocked)
        {
            _soundControler.PlaySound(_soundControler.ClickIn);
        }
        else if ((Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(_keyBinding[9])) && !Constants.EscapeLocked)
        {
            _soundControler.PlaySound(_soundControler.ClickOut);
            _currentScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
            if (Constants.InputLayer > 0)
            {
                var gameObjectToDestroy = GameObject.Find(Constants.InputTopLayerNames[Constants.InputTopLayerNames.Count - 1]);
                if (!_currentScene.Paused)
                    _mainCamera.gameObject.GetComponent<CameraBhv>().Unfocus();
                if (gameObjectToDestroy.name.Contains("Keyboard"))
                    gameObjectToDestroy.transform.GetChild(0).GetComponent<PopupBhv>().ExitPopup();
                else
                    gameObjectToDestroy.GetComponent<PopupBhv>().ExitPopup();
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
                            || IsUnderSprite(_currentInput, hitsInformation))
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
                if (!Constants.OnlyMouseInMenu)
                {
                    Constants.OnlyMouseInMenu = true;
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
                            || IsUnderSprite(_currentInput, hitsInformation))
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

    private void CheckGameKeyboardInputs()
    {
        if (_gameplayControler == null)
            return;
        if (Input.GetKeyDown(_keyBinding[0]))
        {
            _gameplayControler.HardDrop();
        }
        if (Input.GetKey(_keyBinding[1]))
        {
            _gameplayControler.SoftDropHolded();
        }
        if (Input.GetKeyDown(_keyBinding[2]))
        {
            _gameplayControler.Left();
        }
        if (Input.GetKey(_keyBinding[2]))
        {
            _gameplayControler.LeftHolded();
        }
        if (Input.GetKeyUp(_keyBinding[2]) || Input.GetKeyUp(_keyBinding[3]))
        {
            _gameplayControler.DirectionReleased();
        }
        if (Input.GetKeyDown(_keyBinding[3]))
        {
            _gameplayControler.Right();
        }
        if (Input.GetKey(_keyBinding[3]))
        {
            _gameplayControler.RightHolded();
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
        if (Input.anyKeyDown || Input.anyKey)
        {
            for (int i = 0; i < _keyBinding.Count; ++i)
            {
                if (Input.GetKeyDown(_keyBinding[i]) || Input.GetKey(_keyBinding[i]))
                {
                    _gameplayControler.AddFrameKeyPressOrHolded(_inputNames[i]);
                }
            }
        }
        _gameplayControler.UpdateFrameKeysPressOrHolded();
    }

    private void InitMenuKeyboardInputs()
    {
        var allGameObjects = FindObjectsOfType<GameObject>();
        _availableButtons = new List<GameObject>();
        if (allGameObjects.Length > 0)
        {
            for (int i = allGameObjects.Length - 1; i >= 0; i--)
            {
                var buttonBhv = allGameObjects[i].GetComponent<ButtonBhv>();
                if (buttonBhv != null
                    && (buttonBhv.BeginActionDelegate != null || buttonBhv.DoActionDelegate != null || buttonBhv.EndActionDelegate != null)
                    && buttonBhv.Layer == Constants.InputLayer)
                {
                    if (_gameplayControler == true && buttonBhv.Layer == 0)
                        continue;
                    _availableButtons.Add(allGameObjects[i]);
                }
            }
        }
        _currentScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
        if (Constants.InputLayer > _currentInputLayer)
            ResetMenuSelector();
        else
        {
            if (_lastPositions != null && _lastPositions.Count > 1)
            {
                _menuSelector.MoveTo(_lastPositions[_lastPositions.Count - 1]);
                _lastPositions.RemoveAt(_lastPositions.Count - 1);
            }
            else
                ResetMenuSelector();
        }
        _currentInputLayer = Constants.InputLayer;
    }

    private void ResetMenuSelector()
    {
        _menuSelector.GetComponent<MenuSelectorBhv>().Reset(_currentScene.MenuSelectorBasePosition);
        if (_currentScene.MenuSelectorBasePosition == null && !Constants.OnlyMouseInMenu)
            FindNearest(Direction.Down);
    }

    private void CheckMenuKeyboardInputs()
    {
        if (_menuSelector == null)
            return;
        if (_gameplayControler != null)
        {
            if (!_gameplayControler.SceneBhv.Paused)
                return;
        }
        if (_currentInputLayer != Constants.InputLayer)
            InitMenuKeyboardInputs();
        if (Input.GetKeyDown(KeyCode.UpArrow))
            FindNearest(Direction.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            FindNearest(Direction.Down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            FindNearest(Direction.Left);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            FindNearest(Direction.Right);
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(_keyBinding[0]))
            ButtonOnSelector();
    }

    private void FindNearest(Direction direction, bool canRetry = true)
    {
        Constants.OnlyMouseInMenu = false;
        var minDistance = 99.0f;
        GameObject selectedGameObject = null;

        foreach (var button in _availableButtons)
        {
            var precision = 1.0f;
            if ((direction == Direction.Up && button.transform.position.y < _menuSelector.transform.position.y + precision / 2)
                || (direction == Direction.Down && button.transform.position.y > _menuSelector.transform.position.y - precision / 2)
                || (direction == Direction.Left && button.transform.position.x > _menuSelector.transform.position.x - precision / 2)
                || (direction == Direction.Right && button.transform.position.x < _menuSelector.transform.position.x + precision / 2)
                || button.GetComponent<ButtonBhv>().Layer != Constants.InputLayer
                || !Helper.IsVisibleInsideCamera(_mainCamera, button.transform.position))
                continue;
            var currentHorizontalDistance = Mathf.Abs(button.transform.position.x - _menuSelector.transform.position.x);
            var currentVerticalDistance = Mathf.Abs(button.transform.position.y - _menuSelector.transform.position.y);
            var currentDistance = (direction == Direction.Up || direction == Direction.Down) ? ((currentHorizontalDistance * 2) + currentVerticalDistance) : (currentHorizontalDistance + (currentVerticalDistance * 2));
            if (currentDistance < minDistance && currentDistance > precision)
            {
                minDistance = currentDistance;
                selectedGameObject = button;
            }
        }

        if (selectedGameObject != null)
        {
            if (_lastPositions == null)
                _lastPositions = new List<GameObject>();
            if (_lastPositions.Count > 0)
                _lastPositions.RemoveAt(_lastPositions.Count - 1);
            _lastPositions.Add(selectedGameObject);
            _menuSelector.MoveTo(selectedGameObject);
        }
        else if (canRetry == true)
        {
            if (direction == Direction.Up)
                _menuSelector.transform.position += new Vector3(0.0f, -50.0f, 0.0f);
            else if (direction == Direction.Down)
                _menuSelector.transform.position += new Vector3(0.0f, 50.0f, 0.0f);
            else if (direction == Direction.Left)
                _menuSelector.transform.position += new Vector3(50.0f, 0.0f, 0.0f);
            else if (direction == Direction.Right)
                _menuSelector.transform.position += new Vector3(-50.0f, 0.0f, 0.0f);
            FindNearest(direction, canRetry:false);
        }
    }

    private void ButtonOnSelector()
    {
        if (Constants.OnlyMouseInMenu)
            return;
        var minDistance = 99.0f;
        GameObject selectedGameObject = null;

        foreach (var button in _availableButtons)
        {
            var currentDistance = Vector2.Distance(button.transform.position, _menuSelector.transform.position);
            if (currentDistance < minDistance && currentDistance < 1.0f)
            {
                minDistance = currentDistance;
                selectedGameObject = button;
            }
        }

        if (selectedGameObject != null)
        {
            Constants.LastEndActionClickedName = selectedGameObject.name;
            var buttonBhv = selectedGameObject.GetComponent<ButtonBhv>();
            buttonBhv.BeginAction(selectedGameObject.transform.position);
            buttonBhv.DoAction(selectedGameObject.transform.position);
            buttonBhv.EndAction(selectedGameObject.transform.position);
            
        }
    }
}
