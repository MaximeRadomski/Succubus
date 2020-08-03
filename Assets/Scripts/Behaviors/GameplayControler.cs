using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControler : MonoBehaviour
{
    public float GravityDelay;
    public GameObject CurrentPiece;

    private int _timeDirectionHolded;
    private float _nextGravityFall;
    private float _lockDelay;
    private float _nextLock;
    private int _allowedMovesBeforeLock;
    private bool _canHold;
    private SceneBhv _sceneBhv;
    private Instantiator _instantiator;
    private GameObject _currentGhost;
    private int _playFieldHeight;
    private int _playFieldWidth;
    private Vector3 _lastCurrentPieceValidPosition;
    private int _lastNbLinesCleared;
    private int _comboCounter;

    private GameObject _spawner;
    private GameObject _holder;
    private List<GameObject> _nextPieces;
    private string _bag;

    private static Transform[,] _playField;

    void Start()
    {
        Init();
        Spawn();
    }

    private void GameOver()
    {
        CurrentPiece.GetComponent<Piece>().IsLocked = true;
        Invoke(nameof(ReloadAfterDelay), 1.0f);
    }

    private void ReloadAfterDelay()
    {
        NavigationService.ReloadScene();
    }

    private void Init()
    {
        GravityDelay = Constants.GravityDelay;
        _lockDelay = Constants.LockDelay;
        _sceneBhv = GetComponent<SceneBhv>();
        _instantiator = GetComponent<Instantiator>();
        SetButtons();
        CurrentPiece = GameObject.Find("T-Hell");
        _spawner = GameObject.Find(Constants.GoSpawnerName);
        _holder = GameObject.Find(Constants.GoHolderName);
        _nextPieces = new List<GameObject>();
        for (int i = 0; i < 5; ++i)
            _nextPieces.Add(GameObject.Find(Constants.GoNextPieceName + i.ToString("D2")));
        SetNextGravityFall();
        SetTimeDirectionHolded();
        _playFieldHeight = Constants.PlayFieldHeight;
        _playFieldWidth = Constants.PlayFieldWidth;
        _playField = new Transform[_playFieldWidth, _playFieldHeight];
    }

    public void SetGravity(int level)
    {
        GravityDelay = Constants.GravityDelay;
        if (level == 19)
        {
            GravityDelay = 0.0f;
        }
        else if (level >= 20)
        {
            GravityDelay = -1.0f;
        }
        else
        {
            for (int i = 0; i < level - 1; ++i)
            {
                GravityDelay *= 1.0f - (Constants.GravityDelay / (float)Constants.GravityDivider);
                GravityDelay = (float)Math.Round((Decimal)GravityDelay, 5, MidpointRounding.AwayFromZero);
            }
        }
        //Debug.Log("\t[DEBUG]\tGRAVITY = \"" + level + ": " + GravityDelay + "\"");
    }

    private void SetNextGravityFall()
    {
        if (CurrentPiece != null && CurrentPiece.GetComponent<Piece>().IsLocked)
        {
            //Just in case, just because
            _nextGravityFall = Time.time - GravityDelay;
            return;
        }            
        _nextGravityFall = Time.time + GravityDelay;
    }

    private void SetTimeDirectionHolded()
    {
        _timeDirectionHolded = 0;
    }

    private void SetButtons()
    {
        LookForAllPossibleButton(Constants.GoButtonLeftName, Left, 0);
        LookForAllPossibleButton(Constants.GoButtonLeftName, LeftHolded, 1);
        LookForAllPossibleButton(Constants.GoButtonLeftName, DirectionReleased, 2);
        LookForAllPossibleButton(Constants.GoButtonRightName, Right, 0);
        LookForAllPossibleButton(Constants.GoButtonRightName, RightHolded, 1);
        LookForAllPossibleButton(Constants.GoButtonRightName, DirectionReleased, 2);
        LookForAllPossibleButton(Constants.GoButtonDownName, SoftDropHolded, 1);
        LookForAllPossibleButton(Constants.GoButtonHoldName, Hold, 0);
        LookForAllPossibleButton(Constants.GoButtonDropName, HardDrop, 0);
        LookForAllPossibleButton(Constants.GoButtonAntiClockName, AntiClock, 0);
        LookForAllPossibleButton(Constants.GoButtonClockName, Clock, 0);
        LookForAllPossibleButton(Constants.GoButtonItemName, Item, 0);
        LookForAllPossibleButton(Constants.GoButtonSpecialName, Special, 0);

    }

    private void LookForAllPossibleButton(string name, ButtonBhv.ActionDelegate actionDelegate, int inputType)
    {
        for (int i = 1; i <= 16; ++i)
        {
            var tmp = GameObject.Find(name + i.ToString("D2"));
            if (tmp == null)
                break;
            if (inputType == 0)
                tmp.GetComponent<ButtonBhv>().BeginActionDelegate = actionDelegate;
            else if (inputType == 1)
                tmp.GetComponent<ButtonBhv>().DoActionDelegate = actionDelegate;
        }
    }

    private void SetBag()
    {
        var tmpStr = "IJLOSTZ";
        if (_bag == null)
            _bag = "";

        while (tmpStr.Length > 0 || _bag.Length <= 7)
        {
            if (tmpStr.Length <= 0)
                tmpStr = "IJLOSTZ";
            var i = UnityEngine.Random.Range(0, tmpStr.Length);
            if (_bag == "" && (tmpStr[i] == 'S' || tmpStr[i] == 'Z'))
                continue;
            _bag += tmpStr[i].ToString();
            tmpStr = tmpStr.Remove(i, 1);
        }
    }

    private void Spawn()
    {
        if (_bag == null || _bag.Length <= 7)
            SetBag();
        CurrentPiece = _instantiator.NewPiece(_bag.Substring(0, 1), Nature.Hell.ToString(), _spawner.transform.position);
        _currentGhost = _instantiator.NewPiece(_bag.Substring(0, 1), Nature.Hell + "Ghost", _spawner.transform.position);
        if (!IsPiecePosValid(CurrentPiece))
        {
            GameOver();
        }
        DropGhost();
        _bag = _bag.Remove(0, 1);
        UpdateNextPieces();
        _allowedMovesBeforeLock = 0;
        _canHold = true;
        SetNextGravityFall();
        _sceneBhv.OnNewPiece();
    }

    private void UpdateNextPieces()
    {
        for (int i = 0; i < 5; ++i)
        {
            if (_nextPieces[i].transform.childCount > 0)
                Destroy(_nextPieces[i].transform.GetChild(0).gameObject);
        }
        for (int i = 0; i < 5; ++i)
        {
            var tmpPiece = _instantiator.NewPiece(_bag.Substring(i, 1), Nature.Hell.ToString(), _nextPieces[i].transform.position, keepSpawnerX: i > 0 ? true : false);
            tmpPiece.transform.SetParent(_nextPieces[i].transform);
        }
    }

    private void AddToPlayField(GameObject piece)
    {
        foreach (Transform child in piece.transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);

            _playField[roundedX, roundedY] = child;
        }
    }

    void Update()
    {
        if (_sceneBhv.Paused)
            return;
        CheckKeyboardInputs();
        if (GravityDelay >= 0.0f && Time.time >= _nextGravityFall)
        {
            GravityFall();
        }
        else if (GravityDelay <= 0.0f)
        {
            GravityStomp();
        }
        if (IsNextGravityFallPossible() == false)
        {
            if (!CurrentPiece.GetComponent<Piece>().IsLocked)
                HandleLock();
        }
        else
        {
            ResetLock();
        }
    }

    private void HandleLock()
    {
        if (_nextLock < 0.0f)
            _nextLock = Time.time + _lockDelay;
        else if (Time.time < _nextLock)
        {
            CurrentPiece.GetComponent<Piece>().HandleOpacityOnLock((_nextLock - Time.time)/_lockDelay);
            if (_allowedMovesBeforeLock >= Constants.NumberOfAllowedMovesBeforeLock)
            {
                Lock();
            }
        }
        else if (_nextLock <= Time.time)
        {
            Lock();
        }
    }

    private void Lock()
    {
        CurrentPiece.GetComponent<Piece>().IsLocked = true;
        CurrentPiece.GetComponent<Piece>().HandleOpacityOnLock(1.0f);
        _nextLock = -1;
        bool isTwtist = false;
        //Looks for Twists
        if (CurrentPiece.GetComponent<Piece>().Letter != "O")
        {
            var nbLocked = 0;
            _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
            CurrentPiece.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
            if (IsPiecePosValid(CurrentPiece) == false)
                ++nbLocked;
            CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
            CurrentPiece.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
            if (IsPiecePosValid(CurrentPiece) == false)
                ++nbLocked;
            CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
            CurrentPiece.transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            if (IsPiecePosValid(CurrentPiece) == false)
                ++nbLocked;
            CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
            isTwtist = nbLocked == 3;
        }
        if (CurrentPiece != null)
        {
            AddToPlayField(CurrentPiece);
        }
        _sceneBhv.OnPieceLocked(isTwtist ? CurrentPiece.GetComponent<Piece>().Letter : null);
        CheckForLines();
    }

    private void ResetLock()
    {
        CurrentPiece.GetComponent<Piece>().HandleOpacityOnLock(1.0f);
        _nextLock = -1;
    }

    private void Left()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        SetTimeDirectionHolded();
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        IsPiecePosValidOrReset();
        DropGhost();
    }

    private void LeftHolded()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        ++_timeDirectionHolded;
        if (_timeDirectionHolded < 10)
            return;
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        IsPiecePosValidOrReset();
        DropGhost();
    }

    private void Right()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        SetTimeDirectionHolded();
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        IsPiecePosValidOrReset();
        DropGhost();
    }

    private void RightHolded()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        ++_timeDirectionHolded;
        if (_timeDirectionHolded < 10)
            return;
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        IsPiecePosValidOrReset();
        DropGhost();
    }

    private void DirectionReleased()
    {
        SetTimeDirectionHolded();
    }

    private void SoftDropHolded()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        if (Time.time < _nextGravityFall - GravityDelay * 0.95f)
            return;
        SetNextGravityFall();
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
        if (IsPiecePosValidOrReset())
            _sceneBhv.OnSoftDrop();
    }

    private void GravityFall()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        SetNextGravityFall();
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        if (GravityDelay > 0.0f)
        {
            CurrentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            IsPiecePosValidOrReset(isGravity: true);
        }
        else
        {
            CurrentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            IsPiecePosValidOrReset(isGravity: true);
            _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
            FadeBlocksOnLastPosition(CurrentPiece);
            CurrentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            IsPiecePosValidOrReset(isGravity: true);
        }
    }

    private void GravityStomp()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        bool hardDropping = true;
        while (hardDropping)
        {
            _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
            CurrentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            if (IsPiecePosValid(CurrentPiece) == false)
            {
                CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
                hardDropping = false;
            }
        }
    }

    private void HardDrop()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        bool hardDropping = true;
        int nbLinesDropped = 0;
        while (hardDropping)
        {
            _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
            CurrentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            if (IsPiecePosValid(CurrentPiece) == false)
            {
                CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
                hardDropping = false;
                CurrentPiece.GetComponent<Piece>().IsLocked = true;
                Invoke(nameof(Lock), Constants.AfterDropDelay);

                string columns = "";
                int yMin = Mathf.RoundToInt(CurrentPiece.transform.position.y);
                foreach (Transform child in CurrentPiece.transform)
                {
                    int x = Mathf.RoundToInt(child.transform.position.x);
                    if (columns.Contains(x.ToString()))
                        continue;
                    HardDropFadeBlocksOnX(x, yMin);
                    columns += x.ToString();
                }
            }
            else
                ++nbLinesDropped;
        }
        _sceneBhv.OnHardDrop(nbLinesDropped);
    }

    private void HardDropFadeBlocksOnX(int x, int yMin)
    {
        for (int y = 19; y >= yMin; --y)
        {
            _instantiator.NewFadeBlock(Nature.Hell, new Vector3(x, y, 0.0f), 1, -1);
        }
    }

    private void FadeBlocksOnLastPosition(GameObject currentPiece)
    {
        foreach (Transform child in currentPiece.transform)
        {
            int x = Mathf.RoundToInt(child.transform.position.x);
            int y = Mathf.RoundToInt(child.transform.position.y);

            _instantiator.NewFadeBlock(Nature.Hell, new Vector3(x, y, 0.0f), 1, -1);
        }
    }

    private void DropGhost()
    {
        bool hardDropping = true;
        if (_currentGhost != null && CurrentPiece != null)
            _currentGhost.transform.position = CurrentPiece.transform.position;
        while (hardDropping)
        {
            var lastCurrentGhostValidPosition = _currentGhost.transform.position;
            _currentGhost.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            if (IsPiecePosValid(_currentGhost) == false)
            {
                _currentGhost.transform.position = lastCurrentGhostValidPosition;
                hardDropping = false;
            }
        }
    }

    private void Clock()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        var currentPieceModel = CurrentPiece.GetComponent<Piece>();
        if (currentPieceModel.Letter == "O")
            return;
        var rotationState = currentPieceModel.RotationState;
        var tries = new List<List<int>>();
        tries.Add(new List<int>() { 0, 0 });
        if (currentPieceModel.Letter != "I")
        {
            if (rotationState == RotationState.O) //O->R
            {
                tries.Add(new List<int>() { -1, 0 });
                tries.Add(new List<int>() { -1, +1 });
                tries.Add(new List<int>() { 0, -2 });
                tries.Add(new List<int>() { -1, -2 });
            }
            else if (rotationState == RotationState.R) //R->D
            {
                tries.Add(new List<int>() { +1, 0 });
                tries.Add(new List<int>() { +1, -1 });
                tries.Add(new List<int>() { 0, +2 });
                tries.Add(new List<int>() { +1, +2 });
            }
            else if (rotationState == RotationState.L) //L->O
            {
                tries.Add(new List<int>() { -1, 0 });
                tries.Add(new List<int>() { -1, -1 });
                tries.Add(new List<int>() { 0, +2 });
                tries.Add(new List<int>() { -1, +2 });
            }
            else if (rotationState == RotationState.D) //D->L
            {
                tries.Add(new List<int>() { +1, 0 });
                tries.Add(new List<int>() { +1, +1 });
                tries.Add(new List<int>() { 0, -2 });
                tries.Add(new List<int>() { +1, -2 });
            }
        }
        else
        {
            if (rotationState == RotationState.O) //O->R
            {
                tries.Add(new List<int>() { -2, 0 });
                tries.Add(new List<int>() { +1, 0 });
                tries.Add(new List<int>() { -2, -1 });
                tries.Add(new List<int>() { +1, +2 });
            }
            else if (rotationState == RotationState.R) //R->D
            {
                tries.Add(new List<int>() { -1, 0 });
                tries.Add(new List<int>() { +2, 0 });
                tries.Add(new List<int>() { -1, +2 });
                tries.Add(new List<int>() { +2, -1 });
            }
            else if (rotationState == RotationState.L) //L->O
            {
                tries.Add(new List<int>() { +1, 0 });
                tries.Add(new List<int>() { -2, 0 });
                tries.Add(new List<int>() { +1, -2 });
                tries.Add(new List<int>() { -2, +1 });
            }
            else if (rotationState == RotationState.D) //D->L
            {
                tries.Add(new List<int>() { +2, 0 });
                tries.Add(new List<int>() { -1, 0 });
                tries.Add(new List<int>() { +2, +1 });
                tries.Add(new List<int>() { -1, -2 });
            }
        }

        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.Rotate(0.0f, 0.0f, -90.0f);
        for (int i = 0; i < 5; ++i)
        {
            CurrentPiece.transform.position += new Vector3(tries[i][0], tries[i][1], 0.0f);
            if (IsPiecePosValid(CurrentPiece))
            {
                if (rotationState == RotationState.O) //O->R
                    currentPieceModel.RotationState = RotationState.R;
                else if (rotationState == RotationState.R) //R->D
                    currentPieceModel.RotationState = RotationState.D;
                else if (rotationState == RotationState.L) //L->O
                    currentPieceModel.RotationState = RotationState.O;
                else if (rotationState == RotationState.D) //D->L
                    currentPieceModel.RotationState = RotationState.L;
                if (_nextLock > 0.0f)
                    ++_allowedMovesBeforeLock;
                ResetLock();
                SetNextGravityFall();
                _currentGhost.transform.Rotate(0.0f, 0.0f, -90.0f);
                DropGhost();
                return;
            }
            else
            {
                CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
            }
        }
        CurrentPiece.transform.Rotate(0.0f, 0.0f, 90.0f);
    }

    private void AntiClock()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        var currentPieceModel = CurrentPiece.GetComponent<Piece>();
        if (currentPieceModel.Letter == "O")
            return;
        var rotationState = currentPieceModel.RotationState;
        var tries = new List<List<int>>();
        tries.Add(new List<int>() { 0, 0 });
        if (currentPieceModel.Letter != "I")
        {
            if (rotationState == RotationState.O) //O->L
            {
                tries.Add(new List<int>() { +1, 0 });
                tries.Add(new List<int>() { +1, +1 });
                tries.Add(new List<int>() { 0, -2 });
                tries.Add(new List<int>() { +1, -2 });
            }
            else if (rotationState == RotationState.R) //R->O
            {
                tries.Add(new List<int>() { +1, 0 });
                tries.Add(new List<int>() { +1, -1 });
                tries.Add(new List<int>() { 0, +2 });
                tries.Add(new List<int>() { +1, +2 });
            }
            else if (rotationState == RotationState.L) //L->D
            {
                tries.Add(new List<int>() { -1, 0 });
                tries.Add(new List<int>() { -1, -1 });
                tries.Add(new List<int>() { 0, +2 });
                tries.Add(new List<int>() { -1, +2 });
            }
            else if (rotationState == RotationState.D) //D->R
            {
                tries.Add(new List<int>() { -1, 0 });
                tries.Add(new List<int>() { -1, +1 });
                tries.Add(new List<int>() { 0, -2 });
                tries.Add(new List<int>() { -1, -2 });
            }
        }
        else
        {
            if (rotationState == RotationState.O) //O->L
            {
                tries.Add(new List<int>() { -1, 0 });
                tries.Add(new List<int>() { +2, 0 });
                tries.Add(new List<int>() { -1, +2 });
                tries.Add(new List<int>() { +2, -1 });
            }
            else if (rotationState == RotationState.R) //R->O
            {
                tries.Add(new List<int>() { +2, 0 });
                tries.Add(new List<int>() { -1, 0 });
                tries.Add(new List<int>() { +2, +1 });
                tries.Add(new List<int>() { -1, -2 });
            }
            else if (rotationState == RotationState.L) //L->D
            {
                tries.Add(new List<int>() { -2, 0 });
                tries.Add(new List<int>() { +1, 0 });
                tries.Add(new List<int>() { -2, -1 });
                tries.Add(new List<int>() { +1, +2 });
            }
            else if (rotationState == RotationState.D) //D->R
            {
                tries.Add(new List<int>() { +1, 0 });
                tries.Add(new List<int>() { -2, 0 });
                tries.Add(new List<int>() { +1, -2 });
                tries.Add(new List<int>() { -2, +1 });
            }
        }

        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.Rotate(0.0f, 0.0f, 90.0f);
        for (int i = 0; i < 5; ++i)
        {
            CurrentPiece.transform.position += new Vector3(tries[i][0], tries[i][1], 0.0f);
            if (IsPiecePosValid(CurrentPiece))
            {
                if (rotationState == RotationState.O) //O->L
                    currentPieceModel.RotationState = RotationState.L;
                else if (rotationState == RotationState.R) //R->O
                    currentPieceModel.RotationState = RotationState.O;
                else if (rotationState == RotationState.L) //L->D
                    currentPieceModel.RotationState = RotationState.D;
                else if (rotationState == RotationState.D) //D->R
                    currentPieceModel.RotationState = RotationState.R;
                if (_nextLock > 0.0f)
                    ++_allowedMovesBeforeLock;
                ResetLock();
                SetNextGravityFall();
                _currentGhost.transform.Rotate(0.0f, 0.0f, 90.0f);
                DropGhost();
                return;
            }
            else
            {
                CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
            }
        }
        CurrentPiece.transform.Rotate(0.0f, 0.0f, -90.0f);
    }

    private void Hold()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked || !_canHold)
            return;
        if (_holder.transform.childCount <= 0)
        {
            var tmpPiece = _instantiator.NewPiece(CurrentPiece.GetComponent<Piece>().Letter, Nature.Hell.ToString(), _holder.transform.position);
            tmpPiece.transform.SetParent(_holder.transform);
            Destroy(CurrentPiece.gameObject);
            if (_currentGhost != null)
                Destroy(_currentGhost);
            Spawn();
            _canHold = false;
        }
        else
        {
            var tmpHolded = _holder.transform.GetChild(0);
            var pieceLetter = tmpHolded.GetComponent<Piece>().Letter;
            var tmpHolding = _instantiator.NewPiece(CurrentPiece.GetComponent<Piece>().Letter, Nature.Hell.ToString(), _holder.transform.position);
            tmpHolding.transform.SetParent(_holder.transform);
            Destroy(CurrentPiece.gameObject);
            Destroy(tmpHolded.gameObject);
            if (_currentGhost != null)
                Destroy(_currentGhost);
            CurrentPiece = _instantiator.NewPiece(pieceLetter, Nature.Hell.ToString(), _spawner.transform.position);
            _currentGhost = _instantiator.NewPiece(pieceLetter, Nature.Hell + "Ghost", _spawner.transform.position);
            if (!IsPiecePosValid(CurrentPiece))
            {
                GameOver();
            }
            DropGhost();
            _allowedMovesBeforeLock = 0;
            SetNextGravityFall();
            _canHold = false;
        }
    }

    private void Item()
    {

    }

    private void Special()
    {

    }

    private bool IsPiecePosValidOrReset(bool isGravity = false)
    {
        if (IsPiecePosValid(CurrentPiece) == false)
        {
            CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
            return false;
        }
        else
        {
            if (_nextLock > 0.0f && !isGravity)
                ++_allowedMovesBeforeLock;
            else if (isGravity)
                _allowedMovesBeforeLock = 0;
            ResetLock();
            return true;
        }
    }

    private bool IsPiecePosValid(GameObject piece)
    {
        foreach (Transform child in piece.transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);

            if (roundedX < 0 || roundedX >= _playFieldWidth
             || roundedY < 0 || roundedY >= _playFieldHeight)
                return false;

            if (_playField[roundedX, roundedY] != null)
                return false;
        }
        return true;
    }

    private bool IsNextGravityFallPossible()
    {
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        CurrentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
        var result = IsPiecePosValid(CurrentPiece);
        CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
        return result;
    }

    #region Lines

    private void CheckForLines()
    {
        if (_currentGhost != null)
            Destroy(_currentGhost);
        int nbLines = 0;
        for (int y = _playFieldHeight - 1; y >= 0; --y)
        {
            if (HasLine(y))
            {
                ++nbLines;
                DeleteLine(y);
            }
        }
        if (nbLines > 0)
        {
            bool isB2B = false;
            if (nbLines > 1 && nbLines == _lastNbLinesCleared)
                isB2B = true;
            _lastNbLinesCleared = nbLines;
            _sceneBhv.OnLinesCleared(nbLines, isB2B);

            ++_comboCounter;
            if (_comboCounter > 1)
                _sceneBhv.OnCombo(_comboCounter);

            _sceneBhv.PopText();
            Invoke(nameof(ClearLineSpace), 0.3f);
        }
        else
        {
            _sceneBhv.OnLinesCleared(nbLines, false);
            _sceneBhv.PopText();
            _comboCounter = 0;
            Spawn();
        }
            
    }

    private bool HasLine(int y)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            if (_playField[x, y] == null)
                return false;
        }
        return true;
    }

    private void DeleteLine(int y)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            _instantiator.NewFadeBlock(Nature.Hell, _playField[x, y].transform.position, 5, 0);
            Destroy(_playField[x, y].gameObject);
            _playField[x, y] = null;
        }
    }

    private void ClearLineSpace()
    {
        int highestBlock = _playFieldHeight - 1;
        for (int y = 0; y < _playFieldHeight; ++y)
        {
            if (y == 0)
                highestBlock = GetHighestBlock();
            if (y > highestBlock)
                break;
            if (HasFullLineSpace(y))
            {
                DropAllAboveLines(y);
                y = -1;
            }
        }
        Spawn();
    }

    public int GetHighestBlock()
    {
        for (int y = _playFieldHeight - 1; y >= 0; --y)
        {
            if (!HasFullLineSpace(y))
                return y;
        }
        return 0;
    }

    private bool HasFullLineSpace(int y)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            if (_playField[x, y] != null)
                return false;
        }
        return true;
    }

    private void DropAllAboveLines(int underY)
    {
        for (int y = underY + 1; y < _playFieldHeight; ++y)
        {
            for (int x = 0; x < _playFieldWidth; ++x)
            {
                if (_playField[x, y] != null)
                {
                    _playField[x, y - 1] = _playField[x, y];
                    _playField[x, y] = null;
                    _playField[x, y - 1].transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                }
            }
        }
    }

    #endregion

    private void CheckKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Hold();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            AntiClock();
        }
        if (Input.GetKey(KeyCode.Keypad2))
        {
            SoftDropHolded();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Left();
        }
        if (Input.GetKey(KeyCode.Keypad4))
        {
            LeftHolded();
        }
        if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            DirectionReleased();
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            Clock();
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Right();
        }
        if (Input.GetKey(KeyCode.Keypad6))
        {
            RightHolded();
        }
        if (Input.GetKeyUp(KeyCode.Keypad6))
        {
            DirectionReleased();
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            HardDrop();
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
