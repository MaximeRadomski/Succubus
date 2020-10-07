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

    private void Start()
    {
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _gameplayControler = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>();
    }

    void Update()
    {
        if (Constants.InputLocked)
            return;
        CheckKeyboardInputs();
        // IF BACK BUTTON //
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _soundControler.PlaySound(_soundControler.ClickIn);
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            _soundControler.PlaySound(_soundControler.ClickOut);
            _currentScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
            if (Constants.InputLayer > 0)
            {
                var gameObjectToDestroy = GameObject.Find(Constants.InputTopLayerNames[Constants.InputTopLayerNames.Count - 1]);
                if (!_currentScene.Paused)
                    Camera.main.gameObject.GetComponent<CameraBhv>().Unfocus();
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
                _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, Camera.main.transform.forward);
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
                _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, Camera.main.transform.forward);
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

    private void CheckKeyboardInputs()
    {
        if (_gameplayControler == null)
            return;
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Space))
        {
            _gameplayControler.Hold();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.B))
        {
            _gameplayControler.AntiClock();
        }
        if (Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.N))
        {
            _gameplayControler.SoftDropHolded();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.G))
        {
            _gameplayControler.Left();
        }
        if (Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.G))
        {
            _gameplayControler.LeftHolded();
        }
        if (Input.GetKeyUp(KeyCode.Keypad4) || Input.GetKeyUp(KeyCode.G))
        {
            _gameplayControler.DirectionReleased();
        }
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.H))
        {
            _gameplayControler.Clock();
        }
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.J))
        {
            _gameplayControler.Right();
        }
        if (Input.GetKey(KeyCode.Keypad6) || Input.GetKey(KeyCode.J))
        {
            _gameplayControler.RightHolded();
        }
        if (Input.GetKeyUp(KeyCode.Keypad6) || Input.GetKeyUp(KeyCode.J))
        {
            _gameplayControler.DirectionReleased();
        }
        if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Y))
        {
            _gameplayControler.HardDrop();
        }
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.T))
        {
            _gameplayControler.Item();
        }
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.U))
        {
            _gameplayControler.Special();
        }
    }

    //void OnGUI()
    //{
    //    Event e = Event.current;
    //    if (e.isKey)
    //    {
    //        Debug.Log("Detected key code: " + e.keyCode);
    //    }
    //}
}
