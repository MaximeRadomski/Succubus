using UnityEngine;
using System.Collections;
using System;

public class SwipeControlerBhv : MonoBehaviour
{
    private Vector3 _touchPosWorld;
    private bool _beginPhase, _doPhase, _endPhase;
    private Vector2 _beginPos;
    private Vector2 _reBeginPos;
    private GameplayControler _gameplayControler;
    private Vector2 _rotationFrontier;
    private float _tapZoneBoundaryHorizontal;
    private float _tapZoneBoundaryVertical;
    private bool _isHoldingDown;
    private int _framesBeforeHoldingDown;
    private Direction _direction;
    private Camera _mainCamera;

    private float _oneTapDistance = 1.0f;
    private float _verticalSensitivity = 1.5f;
    private float _horizontalSensitivity;

    public void Init(GameplayControler gameplayControler, GameObject rotationFrontier)
    {
        _gameplayControler = gameplayControler;
        _rotationFrontier = rotationFrontier.transform.position;
        _tapZoneBoundaryHorizontal = rotationFrontier.GetComponent<BoxCollider2D>().size.x / 2.0f;
        _tapZoneBoundaryVertical = rotationFrontier.GetComponent<BoxCollider2D>().size.y;
        _horizontalSensitivity = PlayerPrefsHelper.GetTouchSensitivity();
        _mainCamera = Helper.GetMainCamera();
    }

    void Update()
    {
        if (Constants.InputLocked || _gameplayControler == null || _gameplayControler.SceneBhv.Paused || _gameplayControler.CurrentPiece == null)
        {
            _beginPos = new Vector3(-99, -99);
            _reBeginPos = _beginPos;
            return;
        }
        // IF SCREEN TOUCH //
        if (Input.touchCount > 0)
        {
            _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ResetBeginPos(touchPosWorld2D);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (_beginPos.x > -90.0f)
                    CheckEndPhase(touchPosWorld2D);
            }
            else
            {
                if (_beginPos.x < -90.0f)
                    ResetBeginPos(touchPosWorld2D);
                CheckDoPhase(touchPosWorld2D);
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
                _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                if (_beginPhase)
                {
                    ResetBeginPos(touchPosWorld2D);
                }
                else if (_endPhase)
                {
                    if (_beginPos.x > -90.0f)
                        CheckEndPhase(touchPosWorld2D);
                }
                else if (_doPhase)
                {
                    if (_beginPos.x < -90.0f)
                        ResetBeginPos(touchPosWorld2D);
                    CheckDoPhase(touchPosWorld2D);
                }
            }
            // ELSE //
            else
                _touchPosWorld = new Vector3(-99, -99, -99);
        }
    }

    private void ResetBeginPos(Vector2 touchPosWorld2D)
    {
        _beginPos = touchPosWorld2D;
        _reBeginPos = _beginPos;
        _framesBeforeHoldingDown = 0;
        _direction = Direction.None;
    }

    private void CheckEndPhase(Vector2 currentPos)
    {
        if (Vector2.Distance(_beginPos, currentPos) <= _oneTapDistance)
        {
            if (((currentPos.x < _rotationFrontier.x - _tapZoneBoundaryHorizontal
                || currentPos.x > _rotationFrontier.x + _tapZoneBoundaryHorizontal)
                && currentPos.y < _rotationFrontier.y)
                || ((currentPos.x < _rotationFrontier.x - _tapZoneBoundaryVertical
                || currentPos.x > _rotationFrontier.x + _tapZoneBoundaryVertical)
                && currentPos.y < _rotationFrontier.y - _tapZoneBoundaryVertical))
                return;
            if (currentPos.x < _rotationFrontier.x)
                _gameplayControler.AntiClock();
            else
                _gameplayControler.Clock();
        }
        else if (Vector2.Distance(new Vector2(0.0f, _reBeginPos.y), new Vector2(0.0f, currentPos.y)) > _verticalSensitivity)
        {
            if (currentPos.y > _reBeginPos.y)
                _gameplayControler.Hold();
            else if (!_isHoldingDown)
                _gameplayControler.HardDrop();
        }
        _isHoldingDown = false;
        _framesBeforeHoldingDown = 0;
        _direction = Direction.None;
    }

    private void CheckDoPhase(Vector2 currentPos)
    {
        if (Vector2.Distance(new Vector2(_reBeginPos.x, 0.0f), new Vector2(currentPos.x, 0.0f)) > _horizontalSensitivity)
            _direction = Direction.Horizontal;
        else if (Vector2.Distance(new Vector2(0.0f, _beginPos.y), new Vector2(0.0f, currentPos.y)) > (_verticalSensitivity / 2))
            _direction = Direction.Vertical;

        if (Vector2.Distance(new Vector2(_reBeginPos.x, 0.0f), new Vector2(currentPos.x, 0.0f)) > _horizontalSensitivity && _direction == Direction.Horizontal)
        {
            var canMimic = Helper.FloatEqualsPrecision(_beginPos.x, _reBeginPos.x, 0.01f);
            if (currentPos.x > _reBeginPos.x)
                _gameplayControler.Right(canMimic);
            else
                _gameplayControler.Left(canMimic);
            //Debug.Log("beginX = " + _beginPos.x + "\treBeginX = " + _reBeginPos.x);
            _reBeginPos = currentPos;
        }
        else if (_isHoldingDown
            || (Vector2.Distance(new Vector2(0.0f, _beginPos.y), new Vector2(0.0f, currentPos.y)) > _verticalSensitivity && currentPos.y < _beginPos.y && _direction == Direction.Vertical))
        {
            ++_framesBeforeHoldingDown;
            if (_framesBeforeHoldingDown < 10)
                return;
            _gameplayControler.SoftDropHolded();
            if (!_isHoldingDown)
            {
                _reBeginPos = currentPos;
                _direction = Direction.None;
                _isHoldingDown = true;
            }
        }
    }
}