using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControler : MonoBehaviour
{
    public float GravityDelay;

    private int _timeDirectionHolded;
    private float _nextGravityFall;
    private float _lockDelay;
    private float _nextLock;
    private int _allowedMovesBeforeLock;
    private SceneBhv _sceneBhv;
    private Instantiator _instantiator;
    private GameObject _currentPiece;
    private int _playFieldHeight;
    private int _playFieldWidth;
    private Vector3 _lastCurrentPieceValidPosition;

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

    private void Init()
    {
        GravityDelay = Constants.GravityDelay;
        _lockDelay = Constants.LockDelay;
        _sceneBhv = GetComponent<SceneBhv>();
        _instantiator = GetComponent<Instantiator>();
        SetButtons();
        _currentPiece = GameObject.Find("T-Hell");
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

    private void SetNextGravityFall()
    {
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
        LookForAllPossibleButton(Constants.GoButtonDownName, DownHolded, 1);
        LookForAllPossibleButton(Constants.GoButtonHoldName, Hold, 0);
        LookForAllPossibleButton(Constants.GoButtonDropName, Drop, 0);
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
            var i = Random.Range(0, tmpStr.Length);
            if (_bag == "" && (tmpStr[i] == 'S' || tmpStr[i] == 'Z'))
                continue;
            _bag += tmpStr[i].ToString();
            tmpStr = tmpStr.Remove(i, 1);
        }
        Debug.Log("\t[DEBUG]\tNew Bag = \"" + _bag + "\"");
    }

    private void Spawn()
    {
        if (_bag == null || _bag.Length <= 7)
            SetBag();
        _currentPiece = _instantiator.NewPiece(_bag.Substring(0, 1), "Hell", _spawner.transform.position);
        _bag = _bag.Remove(0, 1);
        UpdateNextPieces();
        _allowedMovesBeforeLock = 0;
        SetNextGravityFall();
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
            var tmpPiece = _instantiator.NewPiece(_bag.Substring(i, 1), "Hell", _nextPieces[i].transform.position, keepSpawnerX: i > 0 ? true : false);
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
        if (Time.time >= _nextGravityFall)
        {
            GravityFall();
        }
        if (IsNextGravityFallPossible() == false)
        {
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
        _nextLock = -1;
        if (_currentPiece != null)
        {
            AddToPlayField(_currentPiece);
        }
        CheckForLines();
    }

    private void ResetLock()
    {
        _nextLock = -1;
    }

    private void Left()
    {
        if (_currentPiece.GetComponent<Piece>().IsLocked)
            return;
        SetTimeDirectionHolded();
        _lastCurrentPieceValidPosition = _currentPiece.transform.position;
        _currentPiece.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        IsPiecePosValidOrReset();
    }

    private void LeftHolded()
    {
        if (_currentPiece.GetComponent<Piece>().IsLocked)
            return;
        ++_timeDirectionHolded;
        if (_timeDirectionHolded < 10)
            return;
        _lastCurrentPieceValidPosition = _currentPiece.transform.position;
        _currentPiece.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        IsPiecePosValidOrReset();
    }

    private void Right()
    {
        if (_currentPiece.GetComponent<Piece>().IsLocked)
            return;
        SetTimeDirectionHolded();
        _lastCurrentPieceValidPosition = _currentPiece.transform.position;
        _currentPiece.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        IsPiecePosValidOrReset();
    }

    private void RightHolded()
    {
        if (_currentPiece.GetComponent<Piece>().IsLocked)
            return;
        ++_timeDirectionHolded;
        if (_timeDirectionHolded < 10)
            return;
        _lastCurrentPieceValidPosition = _currentPiece.transform.position;
        _currentPiece.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        IsPiecePosValidOrReset();
    }

    private void DirectionReleased()
    {
        SetTimeDirectionHolded();
    }

    private void DownHolded()
    {
        if (_currentPiece.GetComponent<Piece>().IsLocked)
            return;
        if (Time.time < _nextGravityFall - GravityDelay * 0.95f)
            return;
        SetNextGravityFall();
        _lastCurrentPieceValidPosition = _currentPiece.transform.position;
        _currentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
        IsPiecePosValidOrReset();
    }

    private void GravityFall()
    {
        if (_currentPiece.GetComponent<Piece>().IsLocked)
            return;
        SetNextGravityFall();
        _lastCurrentPieceValidPosition = _currentPiece.transform.position;
        _currentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
        IsPiecePosValidOrReset(isGravity: true);
    }

    private void Drop()
    {
        if (_currentPiece.GetComponent<Piece>().IsLocked)
            return;
        bool hardDropping = true;
        while (hardDropping)
        {
            _lastCurrentPieceValidPosition = _currentPiece.transform.position;
            _currentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            if (IsPiecePosValid(_currentPiece) == false)
            {
                _currentPiece.transform.position = _lastCurrentPieceValidPosition;
                hardDropping = false;
                _currentPiece.GetComponent<Piece>().IsLocked = true;
                Invoke(nameof(Lock), Constants.AfterDropDelay);
            }
        }
    }

    private void Clock()
    {
        if (_currentPiece.GetComponent<Piece>().IsLocked)
            return;
        var currentPieceModel = _currentPiece.GetComponent<Piece>();
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

        _lastCurrentPieceValidPosition = _currentPiece.transform.position;
        _currentPiece.transform.Rotate(0.0f, 0.0f, -90.0f);
        for (int i = 0; i < 5; ++i)
        {
            _currentPiece.transform.position += new Vector3(tries[i][0], tries[i][1], 0.0f);
            if (IsPiecePosValid(_currentPiece))
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
                return;
            }
            else
            {
                _currentPiece.transform.position = _lastCurrentPieceValidPosition;
            }
        }
        _currentPiece.transform.Rotate(0.0f, 0.0f, 90.0f);
    }

    private void AntiClock()
    {
        if (_currentPiece.GetComponent<Piece>().IsLocked)
            return;
        _currentPiece.transform.Rotate(0.0f, 0.0f, 90.0f);
    }

    private void Hold()
    {
        if (_holder.transform.childCount <= 0)
        {
            var tmpPiece = _instantiator.NewPiece(_currentPiece.GetComponent<Piece>().Letter, "Hell", _holder.transform.position);
            tmpPiece.transform.SetParent(_holder.transform);
            Destroy(_currentPiece.gameObject);
            Spawn();
        }
        else
        {
            var tmpHolded = _holder.transform.GetChild(0);
            var pieceLetter = tmpHolded.GetComponent<Piece>().Letter;
            var tmpHolding = _instantiator.NewPiece(_currentPiece.GetComponent<Piece>().Letter, "Hell", _holder.transform.position);
            tmpHolding.transform.SetParent(_holder.transform);
            Destroy(_currentPiece.gameObject);
            Destroy(tmpHolded.gameObject);
            _currentPiece = _instantiator.NewPiece(pieceLetter, "Hell", _spawner.transform.position);
            _allowedMovesBeforeLock = 0;
            SetNextGravityFall();
        }
    }

    private void Item()
    {

    }

    private void Special()
    {

    }

    private void IsPiecePosValidOrReset(bool isGravity = false)
    {
        if (IsPiecePosValid(_currentPiece) == false)
        {
            _currentPiece.transform.position = _lastCurrentPieceValidPosition;
        }
        else
        {
            if (_nextLock > 0.0f && !isGravity)
                ++_allowedMovesBeforeLock;
            else if (isGravity)
                _allowedMovesBeforeLock = 0;
            ResetLock();
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
        _lastCurrentPieceValidPosition = _currentPiece.transform.position;
        _currentPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
        var result = IsPiecePosValid(_currentPiece);
        _currentPiece.transform.position = _lastCurrentPieceValidPosition;
        return result;
    }

    #region Lines

    private void CheckForLines()
    {
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
            Invoke(nameof(ClearLineSpace), 0.4f);
        else
            Spawn();
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

    private int GetHighestBlock()
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
            DownHolded();
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
            Drop();
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
