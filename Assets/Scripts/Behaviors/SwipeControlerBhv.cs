using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SwipeControlerBhv : MonoBehaviour
{
    private Vector3 _touchPosWorld;
    private bool _beginPhase, _doPhase, _endPhase;
    private List<Vector2> _beginPos;
    private List<Vector2> _reBeginPos;
    private int _reBeginsCount;
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
        _beginPos = new List<Vector2>();
        _reBeginPos = new List<Vector2>();
    }

    void Update()
    {
        if (Cache.InputLocked || _gameplayControler == null || _gameplayControler.SceneBhv.Paused || _gameplayControler.CurrentPiece == null)
        {
            //for (int i = 0; i < _beginPos.Count; ++i)
            //    if (_beginPos[i].x > -99) _beginPos[i] = new Vector3(-99, -99);
            //for (int i = 0; i < _reBeginPos.Count; ++i)
            //    if (_reBeginPos[i].x > -99) _reBeginPos[i] = new Vector3(-99, -99);
            if (_beginPos.Count > 0)
                _beginPos.Clear();
            if (_reBeginPos.Count > 0)
                _reBeginPos.Clear();
            return;
        }
        // IF SCREEN TOUCH //
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.GetTouch(i).position);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    ResetBeginPos(i, touchPosWorld2D);
                }
                else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                {
                    if (_beginPos.Count > i && _beginPos[i] != null)
                        CheckEndPhase(i, touchPosWorld2D);
                }
                else
                {
                    if (_beginPos.Count <= i || _beginPos[i] == null)
                        ResetBeginPos(i, touchPosWorld2D);
                    CheckDoPhase(i, touchPosWorld2D);
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
                _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                if (_beginPhase)
                {
                    ResetBeginPos(0, touchPosWorld2D);
                }
                else if (_endPhase)
                {
                    if (_beginPos.Count > 0 && _beginPos[0] != null)
                        CheckEndPhase(0, touchPosWorld2D);
                }
                else if (_doPhase)
                {
                    if (_beginPos.Count <= 0 || _beginPos[0] == null)
                        ResetBeginPos(0, touchPosWorld2D);
                    CheckDoPhase(0, touchPosWorld2D);
                }
            }
            // ELSE //
            else
                _touchPosWorld = new Vector3(-99, -99, -99);
        }
    }

    private void ResetBeginPos(int id, Vector2 touchPosWorld2D)
    {
        while (_beginPos.Count <= id)
            _beginPos.Add(new Vector2());
        _beginPos[id] = touchPosWorld2D;
        while (_reBeginPos.Count <= id)
            _reBeginPos.Add(new Vector2());
        _reBeginPos[id] = touchPosWorld2D;
        _reBeginsCount = 0;
        _framesBeforeHoldingDown = 0;
        _direction = Direction.None;
    }

    private void CheckEndPhase(int id, Vector2 currentPos)
    {
        if (Vector2.Distance(_beginPos[id], currentPos) <= _oneTapDistance)
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
        else if (Vector2.Distance(new Vector2(0.0f, _reBeginPos[id].y), new Vector2(0.0f, currentPos.y)) > _verticalSensitivity)
        {
            if (currentPos.y > _reBeginPos[id].y)
                _gameplayControler.Hold();
            else if (!_isHoldingDown)
                _gameplayControler.HardDrop();
        }
        _isHoldingDown = false;
        _framesBeforeHoldingDown = 0;
        _direction = Direction.None;
        _beginPos.RemoveAt(id);
    }

    private void CheckDoPhase(int id, Vector2 currentPos)
    {
        if (Vector2.Distance(new Vector2(_reBeginPos[id].x, 0.0f), new Vector2(currentPos.x, 0.0f)) > _horizontalSensitivity)
            _direction = Direction.Horizontal;
        else if (Vector2.Distance(new Vector2(0.0f, _beginPos[id].y), new Vector2(0.0f, currentPos.y)) > (_verticalSensitivity / 2))
            _direction = Direction.Vertical;

        if (Vector2.Distance(new Vector2(_reBeginPos[id].x, 0.0f), new Vector2(currentPos.x, 0.0f)) > _horizontalSensitivity && _direction == Direction.Horizontal)
        {
            var canMimic = Helper.FloatEqualsPrecision(_beginPos[id].x, _reBeginPos[id].x, 0.01f);
            if (currentPos.x > _reBeginPos[id].x)
                _gameplayControler.Right(canMimic, _reBeginsCount == 0);
            else
                _gameplayControler.Left(canMimic, _reBeginsCount == 0);
            //Debug.Log("beginX = " + _beginPos.x + "\treBeginX = " + _reBeginPos.x);
            _reBeginPos[id] = currentPos;
            ++_reBeginsCount;
        }
        else if (_isHoldingDown
            || (Vector2.Distance(new Vector2(0.0f, _beginPos[id].y), new Vector2(0.0f, currentPos.y)) > _verticalSensitivity && currentPos.y < _beginPos[id].y && _direction == Direction.Vertical))
        {
            ++_framesBeforeHoldingDown;
            if (_framesBeforeHoldingDown < 10)
                return;
            _gameplayControler.SoftDropHeld();
            if (!_isHoldingDown)
            {
                _reBeginPos[id] = currentPos;
                _direction = Direction.None;
                _isHoldingDown = true;
            }
        }
    }
}