using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControler : MonoBehaviour
{
    public SceneBhv SceneBhv;
    public float GravityDelay;
    public GameObject CurrentPiece;
    public GameObject CurrentGhost;
    public string Bag;
    public Instantiator Instantiator;
    public Character Character;

    private Realm _characterRealm;
    private Realm _levelRealm;

    private int _timeDirectionHolded;
    private float _nextGravityFall;
    private float _lockDelay;
    private float _nextLock;
    private int _allowedMovesBeforeLock;
    private bool _canHold;
    private int _playFieldHeight;
    private int _playFieldWidth;
    private Vector3 _lastCurrentPieceValidPosition;
    private int _lastNbLinesCleared;
    private int _comboCounter;

    private GameObject _spawner;
    private GameObject _holder;
    private GameObject _panelLeft;
    private GameObject _panelRight;
    private GameObject _uiPanelLeft;
    private GameObject _uiPanelRight;
    private GameObject _mainCamera;
    private List<GameObject> _gameplayButtons;
    private List<GameObject> _nextPieces;

    private PlayFieldBhv _playFieldBhv;
    private Special _characterSpecial;
    private Item _characterItem;

    private SoundControlerBhv _soundControler;
    private int _id1Line;
    private int _id2Line;
    private int _id3Line;
    private int _id4Line;
    private int _idCombo;
    private int _idConsecutive;
    private int _idHold;
    private int _idItem;
    private int _idLeftRightDown;
    private int _idLock;
    private int _idPerfect;
    private int _idRotate;
    private int _idSpecial;
    private int _idTwist;
    private int _idGameOver;


    public void StartGameplay(int level, Realm characterRealm, Realm levelRealm)
    {
        Init(level, characterRealm, levelRealm);
        Spawn();
    }

    private void GameOver()
    {
        _soundControler.PlaySound(_idGameOver);
        CurrentPiece.GetComponent<Piece>().IsLocked = true;
        Invoke(nameof(CleanPlayerPrefs), 1.0f);
    }

    public void CleanPlayerPrefs()
    {
        Bag = null;
        _characterSpecial.ResetCooldown();
        PlayerPrefsHelper.SaveBag(Bag);
        PlayerPrefsHelper.SaveHolder(null);
        Destroy(_playFieldBhv.gameObject);
        SceneBhv.OnGameOver();
    }

    private void Init(int level, Realm characterRealm, Realm levelRealm)
    {
        SetGravity(level);
        _characterRealm = characterRealm;
        _levelRealm = levelRealm;
        _lockDelay = Constants.LockDelay;
        SceneBhv = GetComponent<SceneBhv>();
        Instantiator = GetComponent<Instantiator>();
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _panelLeft = GameObject.Find("PanelLeft");
        _panelRight = GameObject.Find("PanelRight");
        _uiPanelLeft = GameObject.Find("UiPanelLeft");
        _uiPanelRight = GameObject.Find("UiPanelRight");
        _mainCamera = GameObject.Find("Main Camera");
        _gameplayButtons = new List<GameObject>();
        PanelsVisuals(PlayerPrefsHelper.GetButtonsLeftPanel(), _panelLeft, isLeft: true);
        PanelsVisuals(PlayerPrefsHelper.GetButtonsRightPanel(), _panelRight, isLeft: false);
        if (PlayerPrefsHelper.GetOrientation() == "Horizontal")
            SetHorizontalOrientation();
        UpdatePanelsPositions();
        SetButtons();
        CurrentPiece = GameObject.Find("T-Hell");
        _spawner = GameObject.Find(Constants.GoSpawnerName);
        _holder = GameObject.Find(Constants.GoHolderName);
        _nextPieces = new List<GameObject>();
        for (int i = 0; i < 5; ++i)
            _nextPieces.Add(GameObject.Find(Constants.GoNextPieceName + i.ToString("D2")));
        SetNextGravityFall();
        SetTimeDirectionHolded();
        _playFieldBhv = GameObject.Find("PlayField").GetComponent<PlayFieldBhv>();
        _playFieldHeight = Constants.PlayFieldHeight;
        _playFieldWidth = Constants.PlayFieldWidth;
        if (_playFieldBhv.Grid != null)
        {
            Bag = PlayerPrefsHelper.GetBag();
            var holding = PlayerPrefsHelper.GetHolder();
            if (!string.IsNullOrEmpty(holding))
            {
                var tmpHolding = Instantiator.NewPiece(holding, _characterRealm.ToString(), _holder.transform.position);
                tmpHolding.transform.SetParent(_holder.transform);
            }
            _playFieldBhv.HideShow(1);
        }
        else
        {
            _playFieldBhv.Grid = new Transform[_playFieldWidth, _playFieldHeight];
        }
        Character = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacter()];
        if ((_characterItem = PlayerPrefsHelper.GetCurrentItem()) != null)
            _characterItem.Init(Character, this);
        _characterSpecial = (Special)Activator.CreateInstance(Type.GetType("Special" + Character.SpecialName.Replace(" ", "").Replace("'", "")));
        _characterSpecial.Init(Character, this);
        UpdateItemAndSpecialVisuals();
    }

    public void UpdateItemAndSpecialVisuals()
    {
        //ITEM
        var currentItemName = PlayerPrefsHelper.GetCurrentItemName();
        if (!string.IsNullOrEmpty(currentItemName))
        {
            for (int i = 1; i <= 16; ++i)
            {
                var tmp = GameObject.Find(Constants.GoButtonItemName + i.ToString("D2"));
                if (tmp == null)
                    break;
                tmp.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsGameplay_" + (_characterRealm.GetHashCode() * 10 + 8));//8 = item in sprite sheet
            }
        }
        else
        {
            for (int i = 1; i <= 16; ++i)
            {
                var tmp = GameObject.Find(Constants.GoButtonItemName + i.ToString("D2"));
                if (tmp == null)
                    break;
                tmp.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsGameplay_" + (_characterRealm.GetHashCode() * 10));
            }
        }
        //SPECIAL
        if (Constants.SelectedCharacterSpecialCooldown <= 0)
        {
            for (int i = 1; i <= 16; ++i)
            {
                var tmp = GameObject.Find(Constants.GoButtonSpecialName + i.ToString("D2"));
                if (tmp == null)
                    break;
                tmp.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsGameplay_" + (_characterRealm.GetHashCode() * 10 + 9));//9 = special in sprite sheet
                tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = null;
            }
        }
        else
        {
            for (int i = 1; i <= 16; ++i)
            {
                var tmp = GameObject.Find(Constants.GoButtonSpecialName + i.ToString("D2"));
                if (tmp == null)
                    break;
                tmp.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsGameplay_" + (_characterRealm.GetHashCode() * 10));
                tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = Constants.SelectedCharacterSpecialCooldown.ToString();
            }

        }
    }

    public void SetHorizontalOrientation()
    {
        var resetRotation = new Quaternion();
        resetRotation.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        _mainCamera.transform.position = Constants._cameraHorizontalGameplayPosition;
        _mainCamera.transform.rotation = resetRotation;
        _mainCamera.transform.Rotate(0.0f, 0.0f, 90.0f);
        _panelLeft.GetComponent<PositionBhv>().Rotated = true;
        _panelRight.GetComponent<PositionBhv>().Rotated = true;
        _uiPanelLeft.transform.rotation = resetRotation;
        _uiPanelLeft.transform.Rotate(0.0f, 0.0f, 90.0f);
        var uiPanelLeftPositionBhv = _uiPanelLeft.GetComponent<PositionBhv>();
        uiPanelLeftPositionBhv.VerticalSide = CameraVerticalSide.TopBorder;
        uiPanelLeftPositionBhv.HorizontalSide = CameraHorizontalSide.LeftBorder;
        uiPanelLeftPositionBhv.XOffset = uiPanelLeftPositionBhv.YOffset / 3;
        uiPanelLeftPositionBhv.YOffset = -2.285f;
        uiPanelLeftPositionBhv.Rotated = true;
        uiPanelLeftPositionBhv.UpdatePositions();
        RotatePanelChildren(_uiPanelLeft);
        _uiPanelRight.transform.rotation = resetRotation;
        _uiPanelRight.transform.Rotate(0.0f, 0.0f, 90.0f);
        var uiPanelRightPositionBhv = _uiPanelRight.GetComponent<PositionBhv>();
        uiPanelRightPositionBhv.VerticalSide = CameraVerticalSide.TopBorder;
        uiPanelRightPositionBhv.HorizontalSide = CameraHorizontalSide.RightBorder;
        uiPanelRightPositionBhv.XOffset = -uiPanelRightPositionBhv.YOffset;
        uiPanelRightPositionBhv.YOffset = -2.285f;
        uiPanelRightPositionBhv.Rotated = true;
        uiPanelRightPositionBhv.UpdatePositions();
        RotatePanelChildren(_uiPanelRight);
    }

    private void UpdatePanelsPositions()
    {
        _panelLeft.GetComponent<PositionBhv>().UpdatePositions();
        _panelRight.GetComponent<PositionBhv>().UpdatePositions();
        _uiPanelLeft.GetComponent<PositionBhv>().UpdatePositions();
        _uiPanelRight.GetComponent<PositionBhv>().UpdatePositions();
    }

    private void RotatePanelChildren(GameObject panel)
    {
        panel.transform.GetChild(0).transform.Rotate(0.0f, 0.0f, -90.0f);
        panel.transform.GetChild(0).transform.position += new Vector3(-Constants.Pixel, 0.0f, 0.0f);
        panel.transform.GetChild(1).transform.Rotate(0.0f, 0.0f, -90.0f);
        panel.transform.GetChild(1).transform.position += new Vector3(Constants.Pixel, 0.0f, 0.0f);
    }

    private void PanelsVisuals(string panelStr, GameObject panel, bool isLeft)
    {
        for (int i = 0; i < panelStr.Length; ++i)
        {
            if (panelStr[i] == '0')
                continue;
            SetGameplayButton(GameObject.Find((isLeft ? "L-" : "R-") + "Add" + i.ToString("D2")), i, Helper.LetterToGameplayButton(panelStr[i]));
        }
    }

    private void SetGameplayButton(GameObject addButton, int buttonId, string gameplayButtonName)
    {
        //Debug.Log("\t[DEBUG]\tgameplayButtonName = " + gameplayButtonName);
        var gameplayButton = Instantiator.NewGameplayButton(gameplayButtonName, addButton.transform.position);
        var spriteName = gameplayButton.GetComponent<SpriteRenderer>().sprite.name;
        var spriteId = int.Parse(spriteName.Substring(spriteName.IndexOf('_') + 1));
        gameplayButton.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsGameplay_" + (_characterRealm.GetHashCode() * 10 + spriteId));
        if (addButton.gameObject.name[0] == 'L')
        {
            _gameplayButtons.Add(gameplayButton);
            gameplayButton.transform.parent = _panelLeft.transform;
            gameplayButton.name = gameplayButton.name + Helper.DoesListContainsSameFromName(_gameplayButtons, gameplayButton.name).ToString("D2");
            var save = PlayerPrefsHelper.GetButtonsLeftPanel();
            save = save.ReplaceChar(buttonId, Helper.GameplayButtonToLetter(gameplayButtonName));
            PlayerPrefsHelper.SaveButtonsLeftPanel(save);
        }
        else
        {
            _gameplayButtons.Add(gameplayButton);
            gameplayButton.transform.parent = _panelRight.transform;
            gameplayButton.name = gameplayButton.name + Helper.DoesListContainsSameFromName(_gameplayButtons, gameplayButton.name).ToString("D2");
            var save = PlayerPrefsHelper.GetButtonsRightPanel();
            save = save.ReplaceChar(buttonId, Helper.GameplayButtonToLetter(gameplayButtonName));
            PlayerPrefsHelper.SaveButtonsRightPanel(save);
        }
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

        _id1Line = _soundControler.SetSound("1Line");
        _id2Line = _soundControler.SetSound("2Line");
        _id3Line = _soundControler.SetSound("3Line");
        _id4Line = _soundControler.SetSound("4Line");
        _idCombo = _soundControler.SetSound("Combo");
        _idConsecutive = _soundControler.SetSound("Consecutive");
        _idHold = _soundControler.SetSound("Hold");
        _idItem = _soundControler.SetSound("Item");
        _idLeftRightDown = _soundControler.SetSound("LeftRightDown");
        _idLock = _soundControler.SetSound("Lock");
        _idPerfect = _soundControler.SetSound("Perfect");
        _idRotate = _soundControler.SetSound("Rotate");
        _idSpecial = _soundControler.SetSound("Special");
        _idTwist = _soundControler.SetSound("Twist");
        _idGameOver = _soundControler.SetSound("GameOver");
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
        //var tmpStr = "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII";
        var tmpStr = "IJLOSTZ";
        if (Bag == null)
            Bag = "";

        while (tmpStr.Length > 0 || Bag.Length <= 7)
        {
            if (tmpStr.Length <= 0)
                tmpStr = "IJLOSTZ";
            var i = UnityEngine.Random.Range(0, tmpStr.Length);
            if (Bag == "" && (tmpStr[i] == 'S' || tmpStr[i] == 'Z'))
                continue;
            Bag += tmpStr[i].ToString();
            tmpStr = tmpStr.Remove(i, 1);
        }
    }

    public void Spawn()
    {
        if (Bag == null || Bag.Length <= 7)
            SetBag();
        CurrentPiece = Instantiator.NewPiece(Bag.Substring(0, 1), _characterRealm.ToString(), _spawner.transform.position);
        CurrentGhost = Instantiator.NewPiece(Bag.Substring(0, 1), _characterRealm + "Ghost", _spawner.transform.position);
        CurrentGhost.GetComponent<Piece>().SetColor((Color)Constants.GetColorFromNature(_characterRealm, int.Parse(PlayerPrefsHelper.GetGhostColor())));
        if (!IsPiecePosValid(CurrentPiece))
        {
            GameOver();
        }
        DropGhost();
        PlayerPrefsHelper.SaveBag(Bag);
        Bag = Bag.Remove(0, 1);
        UpdateNextPieces();
        _allowedMovesBeforeLock = 0;
        _canHold = true;
        SetNextGravityFall();
        ResetLock();
        SceneBhv.OnNewPiece();
        _characterSpecial.OnNewPiece(CurrentPiece);
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
            var tmpPiece = Instantiator.NewPiece(Bag.Substring(i, 1), _characterRealm.ToString(), _nextPieces[i].transform.position, keepSpawnerX: i > 0 ? true : false);
            tmpPiece.transform.SetParent(_nextPieces[i].transform);
        }
    }

    private void AddToPlayField(GameObject piece)
    {
        piece.transform.SetParent(_playFieldBhv.gameObject.transform);
        foreach (Transform child in piece.transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);

            _playFieldBhv.Grid[roundedX, roundedY] = child;
        }
    }

    void Update()
    {
        if (SceneBhv.Paused)
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
        if (CurrentPiece.GetComponent<Piece>().HasBlocksAffectedByGravity)
            AffectGravityOnBlocks(CurrentPiece);
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
            if (isTwtist)
                _soundControler.PlaySound(_idTwist);
        }
        if (CurrentPiece != null)
        {
            AddToPlayField(CurrentPiece);
        }
        SceneBhv.OnPieceLocked(isTwtist ? CurrentPiece.GetComponent<Piece>().Letter : null);
        _characterSpecial.OnPieceLocked(CurrentPiece);
        _soundControler.PlaySound(_idLock);
        CheckForLines();
    }

    private void ResetLock()
    {
        CurrentPiece.GetComponent<Piece>().HandleOpacityOnLock(1.0f);
        _nextLock = -1;
    }

    private void AffectGravityOnBlocks(GameObject piece)
    {
        for (int i = 0; i < piece.transform.childCount; ++i)
        {
            var tmpBlock = piece.transform.GetChild(i);
            var lastTmpBlockPosition = tmpBlock.position;
            tmpBlock.position += new Vector3(0.0f, -1.0f, 0.0f);
            if (IsBlockPosValid(tmpBlock, piece.transform))
            {
                i = -1;
            }
            else
            {
                tmpBlock.position = lastTmpBlockPosition;
            }
        }
    }

    private void Left()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        SetTimeDirectionHolded();
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        if (IsPiecePosValidOrReset())
            _soundControler.PlaySound(_idLeftRightDown);
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
        if (IsPiecePosValidOrReset())
            _soundControler.PlaySound(_idLeftRightDown);
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
        if (IsPiecePosValidOrReset())
            _soundControler.PlaySound(_idLeftRightDown);
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
        if (IsPiecePosValidOrReset())
            _soundControler.PlaySound(_idLeftRightDown);
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
            SceneBhv.OnSoftDrop();
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
        //_soundControler.PlaySound(_idHardDrop);
        SceneBhv.OnHardDrop(nbLinesDropped);
    }

    private void HardDropFadeBlocksOnX(int x, int yMin)
    {
        for (int y = 19; y >= yMin; --y)
        {
            Instantiator.NewFadeBlock(_characterRealm, new Vector3(x, y, 0.0f), 1, -1);
        }
    }

    private void FadeBlocksOnLastPosition(GameObject currentPiece)
    {
        foreach (Transform child in currentPiece.transform)
        {
            int x = Mathf.RoundToInt(child.transform.position.x);
            int y = Mathf.RoundToInt(child.transform.position.y);

            Instantiator.NewFadeBlock(_characterRealm, new Vector3(x, y, 0.0f), 1, -1);
        }
    }

    public void FadeBlocksOnText()
    {
        for (int y = 17; y <= 18; ++y)
        {
            for (int x = 0; x < 10; ++x)
            {
                Instantiator.NewFadeBlock(Character.Realm, new Vector3(x, y, 0.0f), 2, -1);
            }
        }
    }

    private void DropGhost()
    {
        bool hardDropping = true;
        if (CurrentGhost != null && CurrentPiece != null)
            CurrentGhost.transform.position = CurrentPiece.transform.position;
        while (hardDropping)
        {
            var lastCurrentGhostValidPosition = CurrentGhost.transform.position;
            CurrentGhost.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            if (IsPiecePosValid(CurrentGhost) == false)
            {
                CurrentGhost.transform.position = lastCurrentGhostValidPosition;
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
                CurrentGhost.transform.Rotate(0.0f, 0.0f, -90.0f);
                DropGhost();
                _soundControler.PlaySound(_idRotate);
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
                CurrentGhost.transform.Rotate(0.0f, 0.0f, 90.0f);
                DropGhost();
                _soundControler.PlaySound(_idRotate);
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
            var tmpHolding = Instantiator.NewPiece(CurrentPiece.GetComponent<Piece>().Letter, _characterRealm.ToString(), _holder.transform.position);
            tmpHolding.transform.SetParent(_holder.transform);
            PlayerPrefsHelper.SaveHolder(tmpHolding.GetComponent<Piece>().Letter);
            Destroy(CurrentPiece.gameObject);
            if (CurrentGhost != null)
                Destroy(CurrentGhost);
            Spawn();
            _canHold = false;
        }
        else
        {
            var tmpHolded = _holder.transform.GetChild(0);
            var pieceLetter = tmpHolded.GetComponent<Piece>().Letter;
            var tmpHolding = Instantiator.NewPiece(CurrentPiece.GetComponent<Piece>().Letter, _characterRealm.ToString(), _holder.transform.position);
            tmpHolding.transform.SetParent(_holder.transform);
            PlayerPrefsHelper.SaveHolder(tmpHolding.GetComponent<Piece>().Letter);
            Destroy(CurrentPiece.gameObject);
            Destroy(tmpHolded.gameObject);
            if (CurrentGhost != null)
                Destroy(CurrentGhost);
            CurrentPiece = Instantiator.NewPiece(pieceLetter, _characterRealm.ToString(), _spawner.transform.position);
            CurrentGhost = Instantiator.NewPiece(pieceLetter, _characterRealm + "Ghost", _spawner.transform.position);
            CurrentGhost.GetComponent<Piece>().SetColor((Color)Constants.GetColorFromNature(_characterRealm, int.Parse(PlayerPrefsHelper.GetGhostColor())));
            if (!IsPiecePosValid(CurrentPiece))
            {
                GameOver();
            }
            DropGhost();
            _allowedMovesBeforeLock = 0;
            SetNextGravityFall();
            ResetLock();
            _canHold = false;
            _characterSpecial.OnNewPiece(CurrentPiece);
        }
        _soundControler.PlaySound(_idHold);
    }

    private void Item()
    {
        if (_characterItem != null)
        {
            _soundControler.PlaySound(_idItem);
            var result = _characterItem.Activate();
            if (result == true)
                _characterItem = null;
        }
    }

    private void Special()
    {
        if (_characterSpecial.Activate())
            _soundControler.PlaySound(_idSpecial);
        UpdateItemAndSpecialVisuals();
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
            if (!IsBlockPosValid(child, piece.transform))
                return false;
        }
        return true;
    }

    private bool IsBlockPosValid(Transform block, Transform piece)
    {
        int roundedX = Mathf.RoundToInt(block.transform.position.x);
        int roundedY = Mathf.RoundToInt(block.transform.position.y);

        if (roundedX < 0 || roundedX >= _playFieldWidth
            || roundedY < 0 || roundedY >= _playFieldHeight)
            return false;

        if (_playFieldBhv.Grid[roundedX, roundedY] != null)
            return false;

        var nbSamePos = 0;
        foreach (Transform child in piece)
        {
            int childRoundedX = Mathf.RoundToInt(child.transform.position.x);
            int childRoundedY = Mathf.RoundToInt(child.transform.position.y);
            if (roundedX == childRoundedX && roundedY == childRoundedY)
                ++nbSamePos;
        }
        if (nbSamePos > 1)
            return false;

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
        if (CurrentGhost != null)
            Destroy(CurrentGhost);
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
            if (nbLines == 1)
                _soundControler.PlaySound(_id1Line);
            else if (nbLines == 2)
                _soundControler.PlaySound(_id2Line);
            else if (nbLines == 3)
                _soundControler.PlaySound(_id3Line);
            else if (nbLines == 4)
                _soundControler.PlaySound(_id4Line);

            bool isB2B = false;
            if (nbLines > 1 && nbLines == _lastNbLinesCleared)
            {
                isB2B = true;
                _soundControler.PlaySound(_idConsecutive);
            }
            _lastNbLinesCleared = nbLines;
            SceneBhv.OnLinesCleared(nbLines, isB2B);
            _characterSpecial.OnLinesCleared(nbLines, isB2B);

            ++_comboCounter;
            if (_comboCounter > 1)
            {
                _soundControler.PlaySound(_idCombo, 1.0f + ((_comboCounter - 2) * 0.15f));
                SceneBhv.OnCombo(_comboCounter);
            }

            if (GetHighestBlock() == -1) //PERFECT
            {
                _soundControler.PlaySound(_idPerfect);
                SceneBhv.OnPerfectClear();
            }                

            SceneBhv.PopText();
            UpdateItemAndSpecialVisuals();
            Invoke(nameof(ClearLineSpace), 0.3f);
        }
        else
        {
            SceneBhv.OnLinesCleared(nbLines, false);
            SceneBhv.PopText();
            _comboCounter = 0;
            Spawn();
        }
            
    }

    private bool HasLine(int y)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            if (_playFieldBhv.Grid[x, y] == null)
                return false;
        }
        return true;
    }

    private void DeleteLine(int y)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            if (_playFieldBhv.Grid[x, y] == null)
                continue;
            Instantiator.NewFadeBlock(_characterRealm, _playFieldBhv.Grid[x, y].transform.position, 5, 0);
            Destroy(_playFieldBhv.Grid[x, y].gameObject);
            _playFieldBhv.Grid[x, y] = null;
        }
    }

    private void ClearLineSpace()
    {
        int highestBlock = _playFieldHeight - 1;
        for (int y = 0; y < _playFieldHeight; ++y)
        {
            if (y == 0)
                highestBlock = GetHighestBlock();
            if (y > highestBlock || highestBlock == -1)
                break;
            if (HasFullLineSpace(y))
            {
                DropAllAboveLines(y);
                y = -1;
            }
        }
        foreach (Transform child in _playFieldBhv.transform)
        {
            if (child.childCount == 0)
                Destroy(child.gameObject);
        }
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            Spawn();
        else
            DropGhost();
    }

    public int GetHighestBlock()
    {
        for (int y = _playFieldHeight - 1; y >= 0; --y)
        {
            if (!HasFullLineSpace(y))
                return y;
        }
        return -1;
    }

    private bool HasFullLineSpace(int y)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            if (_playFieldBhv.Grid[x, y] != null)
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
                if (_playFieldBhv.Grid[x, y] != null)
                {
                    _playFieldBhv.Grid[x, y - 1] = _playFieldBhv.Grid[x, y];
                    _playFieldBhv.Grid[x, y] = null;
                    _playFieldBhv.Grid[x, y - 1].transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                }
            }
        }
    }

    public void ClearFromTop(int nbRows)
    {
        int start = GetHighestBlock();
        int end = start - (nbRows - 1);
        for (int y = start; y >= end; --y)
        {
            if (y < 0)
                break;
            DeleteLine(y);
        }
        ClearLineSpace();
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
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Item();
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            Special();
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
