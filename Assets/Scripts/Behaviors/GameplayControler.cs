using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameplayControler : MonoBehaviour
{
    public GameSceneBhv SceneBhv;
    public float GravityDelay;
    public GameObject CurrentPiece;
    public GameObject CurrentGhost;
    public string Bag;
    public Instantiator Instantiator;
    public Character Character;
    public bool AttackIncoming;
    public PlayFieldBhv PlayFieldBhv;
    public GameObject ForcedPiece;

    private Realm _characterRealm;
    private Realm _levelRealm;

    private int _timeDirectionHolded;
    private float _nextGravityFall;
    private float _lockDelay;
    private float _nextLock;
    private int _allowedMovesBeforeLock;
    private bool _canHold;
    private bool _hasInit;
    private int _playFieldHeight;
    private int _playFieldWidth;
    private Vector3 _lastCurrentPieceValidPosition;
    private int _lastNbLinesCleared;
    private int _comboCounter;
    private int _leftHolded, _rightHolded;

    private GameObject _spawner;
    private GameObject _holder;
    private GameObject _panelLeft;
    private GameObject _panelRight;
    private GameObject _uiPanelLeft;
    private GameObject _uiPanelRight;
    private GameObject _mainCamera;
    private List<GameObject> _gameplayButtons;
    private List<GameObject> _nextPieces;
    private CharacterInstanceBhv _characterInstanceBhv;
    private Piece _forcedPieceModel;

    private Special _characterSpecial;
    private Item _characterItem;
    private List<Vector3> _currentGhostPiecesOriginalPos;
    private GameplayChoice _gameplayChoice;
    private Color _ghostColor;

    private SoundControlerBhv _soundControler;
    private MusicControlerBhv _musicControler;
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
    private int _idDarkRows;
    private int _idGarbageRows;
    private int _idLightRows;
    private int _idEmptyRows;
    private int _idCleanRows;
    private int _idVisionBlock;


    public void StartGameplay(int level, Realm characterRealm, Realm levelRealm)
    {
        Init(level, characterRealm, levelRealm);
        Spawn();
        if (Constants.NameLastScene == Constants.SettingsScene)
            SceneBhv.PauseOrPrevious();
    }

    private void GameOver()
    {
        _soundControler.PlaySound(_idGameOver);
        _musicControler.Stop();
        CurrentPiece.GetComponent<Piece>().IsLocked = true;
        Constants.InputLocked = true;
        _characterInstanceBhv.TakeDamage();
        _characterInstanceBhv.Die();
        Invoke(nameof(CleanPlayerPrefs), 1.0f);
        StartCoroutine(Helper.ExecuteAfterDelay(1.0f, () =>
        {
            SceneBhv.OnGameOver();
            return true;
        }));
    }

    public void CleanPlayerPrefs()
    {
        Bag = null;
        _characterSpecial.ResetCooldown();
        PlayerPrefsHelper.SaveBag(Bag);
        PlayerPrefsHelper.SaveHolder(null);
        Destroy(PlayFieldBhv.gameObject);
        Constants.InputLocked = false;
    }

    private void Init(int level, Realm characterRealm, Realm levelRealm)
    {
        if (_hasInit)
            return;
        SetGravity(level);
        _characterRealm = characterRealm;
        _levelRealm = levelRealm;
        _lockDelay = Constants.LockDelay;
        SceneBhv = GetComponent<GameSceneBhv>();
        Instantiator = GetComponent<Instantiator>();
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _musicControler = GameObject.Find(Constants.GoMusicControler).GetComponent<MusicControlerBhv>();
        _panelLeft = GameObject.Find("PanelLeft");
        _panelRight = GameObject.Find("PanelRight");
        _uiPanelLeft = GameObject.Find("UiPanelLeft");
        _uiPanelRight = GameObject.Find("UiPanelRight");
        _mainCamera = GameObject.Find("Main Camera");
        _characterInstanceBhv = GameObject.Find(Constants.GoCharacterInstance).GetComponent<CharacterInstanceBhv>();
        _gameplayButtons = new List<GameObject>();
        _ghostColor = (Color)Constants.GetColorFromNature(_characterRealm, int.Parse(PlayerPrefsHelper.GetGhostColor()));
        PanelsVisuals(PlayerPrefsHelper.GetButtonsLeftPanel(), _panelLeft, isLeft: true);
        PanelsVisuals(PlayerPrefsHelper.GetButtonsRightPanel(), _panelRight, isLeft: false);
        _gameplayChoice = PlayerPrefsHelper.GetGameplayChoice();
        if (_gameplayChoice != GameplayChoice.Buttons)
            SetSwipeGameplayChoice(_gameplayChoice);
        else
            UpdatePanelsPositions();
        SetButtons();

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
        _idDarkRows = _soundControler.SetSound("DarkRows");
        _idGarbageRows = _soundControler.SetSound("GarbageRows");
        _idLightRows = _soundControler.SetSound("LightRows");
        _idEmptyRows = _soundControler.SetSound("EmptyRows");
        _idCleanRows = _soundControler.SetSound("CleanRows");
        _idVisionBlock = _soundControler.SetSound("VisionBlock");

        _spawner = GameObject.Find(Constants.GoSpawnerName);
        _holder = GameObject.Find(Constants.GoHolderName);
        _nextPieces = new List<GameObject>();
        for (int i = 0; i < 5; ++i)
            _nextPieces.Add(GameObject.Find(Constants.GoNextPieceName + i.ToString("D2")));
        SetNextGravityFall();
        SetTimeDirectionHolded();
        PlayFieldBhv = GameObject.Find("PlayField").GetComponent<PlayFieldBhv>();
        _playFieldHeight = Constants.PlayFieldHeight;
        _playFieldWidth = Constants.PlayFieldWidth;
        if (PlayFieldBhv.Grid != null)
        {
            Bag = PlayerPrefsHelper.GetBag();
            var holding = PlayerPrefsHelper.GetHolder();
            if (!string.IsNullOrEmpty(holding))
            {
                var tmpHolding = Instantiator.NewPiece(holding, _characterRealm.ToString(), _holder.transform.position);
                tmpHolding.transform.SetParent(_holder.transform);
            }
            PlayFieldBhv.HideShow(1);
        }
        else
        {
            PlayFieldBhv.Grid = new Transform[_playFieldWidth, _playFieldHeight];
        }
        Character = SceneBhv.Character;
        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
            _characterItem = ItemsData.GetItemFromName(ItemsData.CommonItemsNames[2]);
        else
            _characterItem = PlayerPrefsHelper.GetCurrentItem();
        _characterSpecial = (Special)Activator.CreateInstance(Type.GetType("Special" + Character.SpecialName.Replace(" ", "").Replace("'", "")));
        _characterSpecial.Init(Character, this);
        _characterInstanceBhv.Spawn();
        UpdateItemAndSpecialVisuals();
        _hasInit = true;
    }

    public void UpdateItemAndSpecialVisuals()
    {
        //ITEM
        if (_characterItem != null && Constants.CurrentItemCooldown <= 0)
        {
            for (int i = 1; i <= 16; ++i)
            {
                GameObject tmp = null;
                if (_gameplayChoice == GameplayChoice.Buttons)
                    tmp = GameObject.Find(Constants.GoButtonItemName + i.ToString("D2"));
                else
                {
                    tmp = GameObject.Find(Constants.GoButtonItemName + "Swipe");
                    i = 16;
                }
                if (tmp == null)
                    break;
                tmp.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsGameplay_" + (_characterRealm.GetHashCode() * 10 + 8));//8 = item in sprite sheet
                var beforeText = tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text;
                tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = null;
                tmp.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Items_" + _characterItem.Id.ToString("00"));
                if (beforeText != tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text)
                    tmp.GetComponent<IconInstanceBhv>().Pop();
            }
        }
        else
        {
            for (int i = 1; i <= 16; ++i)
            {
                GameObject tmp = null;
                if (_gameplayChoice == GameplayChoice.Buttons)
                    tmp = GameObject.Find(Constants.GoButtonItemName + i.ToString("D2"));
                else
                {
                    tmp = GameObject.Find(Constants.GoButtonItemName + "Swipe");
                    i = 16;
                }
                if (tmp == null)
                    break;
                tmp.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsGameplay_" + (_characterRealm.GetHashCode() * 10));
                var beforeText = tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text;
                if (_characterItem != null)
                    tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = Constants.CurrentItemCooldown.ToString();
                else
                    tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "";
                tmp.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
                if (beforeText != tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text)
                    tmp.transform.GetChild(0).GetComponent<IconInstanceBhv>().Pop();
            }
        }
        //SPECIAL
        if (Constants.SelectedCharacterSpecialCooldown <= 0)
        {
            for (int i = 1; i <= 16; ++i)
            {
                GameObject tmp = null;
                if (_gameplayChoice == GameplayChoice.Buttons)
                    tmp = GameObject.Find(Constants.GoButtonSpecialName + i.ToString("D2"));
                else
                {
                    tmp = GameObject.Find(Constants.GoButtonSpecialName + "Swipe");
                    i = 16;
                }
                if (tmp == null)
                    break;
                tmp.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsGameplay_" + (_characterRealm.GetHashCode() * 10 + 9));//9 = special in sprite sheet
                var beforeText = tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text;
                tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = null;
                if (beforeText != tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text)
                    tmp.GetComponent<IconInstanceBhv>().Pop();
            }
        }
        else
        {
            for (int i = 1; i <= 16; ++i)
            {
                GameObject tmp = null;
                if (_gameplayChoice == GameplayChoice.Buttons)
                    tmp = GameObject.Find(Constants.GoButtonSpecialName + i.ToString("D2"));
                else
                {
                    tmp = GameObject.Find(Constants.GoButtonSpecialName + "Swipe");
                    i = 16;
                }
                if (tmp == null)
                    break;
                tmp.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsGameplay_" + (_characterRealm.GetHashCode() * 10));
                var beforeText = tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text;
                tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = Constants.SelectedCharacterSpecialCooldown.ToString();
                if (beforeText != tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text)
                    tmp.transform.GetChild(0).GetComponent<IconInstanceBhv>().Pop();
            }

        }
    }

    public void SetSwipeGameplayChoice(GameplayChoice gameplayChoice)
    {
        var mult = 1.0f;
        if (gameplayChoice == GameplayChoice.SwipesLeftHanded)
            mult = -mult;
        var uiPanelLeftPositionBhv = _uiPanelLeft.GetComponent<PositionBhv>();
        uiPanelLeftPositionBhv.HorizontalSide = gameplayChoice == GameplayChoice.SwipesRightHanded ? CameraHorizontalSide.LeftBorder : CameraHorizontalSide.RightBorder;
        uiPanelLeftPositionBhv.XOffset = 2.285f * mult;
        uiPanelLeftPositionBhv.UpdatePositions();        
        var uiPanelRightPositionBhv = _uiPanelRight.GetComponent<PositionBhv>();
        uiPanelRightPositionBhv.HorizontalSide = gameplayChoice == GameplayChoice.SwipesRightHanded ? CameraHorizontalSide.LeftBorder : CameraHorizontalSide.RightBorder;
        uiPanelRightPositionBhv.XOffset = 2.285f * mult;
        uiPanelRightPositionBhv.UpdatePositions();
        _panelLeft.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
        _panelLeft.GetComponent<PositionBhv>().enabled = false;
        _panelRight.transform.position = new Vector3(-30.0f, 30.0f, 0.0f);
        _panelRight.GetComponent<PositionBhv>().enabled = false;
        var panelSwipe = GameObject.Find("PanelSwipe");
        panelSwipe.GetComponent<PositionBhv>().HorizontalSide = gameplayChoice == GameplayChoice.SwipesRightHanded ? CameraHorizontalSide.RightBorder : CameraHorizontalSide.LeftBorder;
        panelSwipe.GetComponent<PositionBhv>().XOffset *= mult;
        panelSwipe.GetComponent<PositionBhv>().UpdatePositions();
        panelSwipe.GetComponent<SwipeControlerBhv>().enabled = true;
        panelSwipe.GetComponent<SwipeControlerBhv>().Init(this, panelSwipe);
        panelSwipe.transform.GetChild(0).GetComponent<ButtonBhv>().EndActionDelegate = Item;
        panelSwipe.transform.GetChild(1).GetComponent<ButtonBhv>().EndActionDelegate = Special;
    }

    private void UpdatePanelsPositions()
    {
        _panelLeft.GetComponent<PositionBhv>().UpdatePositions();
        _panelRight.GetComponent<PositionBhv>().UpdatePositions();
        _uiPanelLeft.GetComponent<PositionBhv>().UpdatePositions();
        _uiPanelRight.GetComponent<PositionBhv>().UpdatePositions();
    }

    //private void RotatePanelChildren(GameObject panel)
    //{
    //    panel.transform.GetChild(0).transform.Rotate(0.0f, 0.0f, -90.0f);
    //    panel.transform.GetChild(0).transform.position += new Vector3(-Constants.Pixel, 0.0f, 0.0f);
    //    panel.transform.GetChild(1).transform.Rotate(0.0f, 0.0f, -90.0f);
    //    panel.transform.GetChild(1).transform.position += new Vector3(Constants.Pixel, 0.0f, 0.0f);
    //}

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
            int levelAfter20 = level - 20;
            _lockDelay = Constants.LockDelay - (Constants.LockDelay * 0.04f * levelAfter20);
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
        LookForAllPossibleButton(Constants.GoButtonLeftName, () => { Left(); }, 0);
        LookForAllPossibleButton(Constants.GoButtonLeftName, LeftHolded, 1);
        LookForAllPossibleButton(Constants.GoButtonLeftName, DirectionReleased, 2);
        LookForAllPossibleButton(Constants.GoButtonRightName, () => { Right(); }, 0);
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
        var tmpLastPiece = CurrentPiece;
        if (tmpLastPiece != null)
            tmpLastPiece.GetComponent<Piece>().AskDisable();
        CurrentPiece = Instantiator.NewPiece(Bag.Substring(0, 1), _characterRealm.ToString(), _spawner.transform.position);
        CurrentGhost = Instantiator.NewPiece(Bag.Substring(0, 1), _characterRealm + "Ghost", _spawner.transform.position);
        CurrentGhost.GetComponent<Piece>().SetColor(_ghostColor);
        if (_currentGhostPiecesOriginalPos != null)
            _currentGhostPiecesOriginalPos.Clear();
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
        SceneBhv.OnNewPiece(tmpLastPiece);
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
        if (PlayFieldBhv == null)
            return;
        piece.transform.SetParent(PlayFieldBhv.gameObject.transform);
        foreach (Transform child in piece.transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);

            PlayFieldBhv.Grid[roundedX, roundedY] = child;
        }
    }

    void Update()
    {
        if (!_hasInit || SceneBhv.Paused)
            return;
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
            CurrentPiece.GetComponent<Piece>().HandleOpacityOnLock(((_nextLock - Time.time)/_lockDelay) + 0.25f);
            if (_allowedMovesBeforeLock >= Constants.NumberOfAllowedMovesBeforeLock && !CurrentPiece.GetComponent<Piece>().IsLocked)
            {
                Lock();
            }
        }
        else if (_nextLock <= Time.time && !CurrentPiece.GetComponent<Piece>().IsLocked)
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
            if (Character.CanDoubleJump)
                CurrentPiece.GetComponent<Piece>().DoubleJump();
        }
        SceneBhv.OnPieceLocked(isTwtist ? CurrentPiece.GetComponent<Piece>().Letter : null);
        _characterSpecial.OnPieceLocked(CurrentPiece);
        _soundControler.PlaySound(_idLock);
        CheckForLightRows();
        CheckForVisionBlocks();
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
            if (IsBlockPosValid(tmpBlock, i, piece.transform))
            {
                i = -1;
            }
            else
            {
                tmpBlock.position = lastTmpBlockPosition;
            }
        }
    }

    public void Left(bool mimicPossible = true)
    {
        _leftHolded = _rightHolded = 0;
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        SetTimeDirectionHolded();
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        if (IsPiecePosValidOrReset(mimicPossible: mimicPossible || CurrentPiece.GetComponent<Piece>().IsMimic))
            _soundControler.PlaySound(_idLeftRightDown);
        DropGhost();
    }

    public void LeftHolded()
    {
        ++_leftHolded;
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        ++_timeDirectionHolded;
        if (_timeDirectionHolded < 10)
            return;
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        if (IsPiecePosValidOrReset(mimicPossible:CurrentPiece.GetComponent<Piece>().IsMimic) && _rightHolded == 0)
            _soundControler.PlaySound(_idLeftRightDown);
        DropGhost();
    }

    public void Right(bool mimicPossible = true)
    {
        _rightHolded = _leftHolded = 0;
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        SetTimeDirectionHolded();
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        if (IsPiecePosValidOrReset(mimicPossible: mimicPossible || CurrentPiece.GetComponent<Piece>().IsMimic))
            _soundControler.PlaySound(_idLeftRightDown);
        DropGhost();
    }

    public void RightHolded()
    {
        ++_rightHolded;
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        ++_timeDirectionHolded;
        if (_timeDirectionHolded < 10)
            return;
        _lastCurrentPieceValidPosition = CurrentPiece.transform.position;
        FadeBlocksOnLastPosition(CurrentPiece);
        CurrentPiece.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        if (IsPiecePosValidOrReset(mimicPossible:CurrentPiece.GetComponent<Piece>().IsMimic) && _leftHolded == 0)
            _soundControler.PlaySound(_idLeftRightDown);
        DropGhost();
    }

    public void DirectionReleased()
    {
        SetTimeDirectionHolded();
    }

    public void SoftDropHolded()
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
        if (CurrentPiece.GetComponent<Piece>().IsLocked || !CurrentPiece.GetComponent<Piece>().IsAffectedByGravity)
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

    private void ForcedPieceHardDrop()
    {
        CurrentPiece.GetComponent<Piece>().IsLocked = false;
        HardDrop();
    }

    public void HardDrop()
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

    public void  DropGhost()
    {
        if (CurrentGhost == null)
            return;
        if (CurrentPiece.GetComponent<Piece>().HasBlocksAffectedByGravity || Character.CanMimic)
        {
            if (_currentGhostPiecesOriginalPos == null || _currentGhostPiecesOriginalPos.Count == 0)
            {
                if (_currentGhostPiecesOriginalPos == null)
                    _currentGhostPiecesOriginalPos = new List<Vector3>();
                foreach (Transform block in CurrentGhost.transform)
                {
                    _currentGhostPiecesOriginalPos.Add(block.transform.localPosition);
                }
            }
            else
            {
                for (int i = 0; i < CurrentGhost.transform.childCount; ++i)
                {
                    var block = CurrentGhost.transform.GetChild(i);
                    block.transform.localPosition = _currentGhostPiecesOriginalPos[i];
                }
            }
        }
        bool hardDropping = true;
        if (CurrentGhost != null && CurrentPiece != null)
            CurrentGhost.transform.position = CurrentPiece.transform.position;
        while (hardDropping)
        {
            var lastCurrentGhostValidPosition = CurrentGhost.transform.position;
            CurrentGhost.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            if (IsPiecePosValid(CurrentGhost, mimicPossible:true) == false)
            {
                CurrentGhost.transform.position = lastCurrentGhostValidPosition;
                if (CurrentPiece.GetComponent<Piece>().HasBlocksAffectedByGravity)
                    AffectGravityOnBlocks(CurrentGhost);
                hardDropping = false;
            }
        }
        DropForcedPiece();
    }

    private void DropForcedPiece()
    {
        if (ForcedPiece == null)
            return;
        bool hardDropping = true;
        ForcedPiece.transform.position = new Vector3(ForcedPiece.transform.position.x, _spawner.transform.position.y + 10.0f + _forcedPieceModel.YFromSpawn, 0.0f);
        while (hardDropping)
        {
            var lastPos = ForcedPiece.transform.position;
            ForcedPiece.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
            if (!IsPiecePosValid(ForcedPiece) || IsPiecesBlocksOverlappingGhost(ForcedPiece))
            {
                ForcedPiece.transform.position = lastPos;
                hardDropping = false;
            }
        }
    }

    public void Clock()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked || CurrentPiece.GetComponent<Piece>().IsMimic)
            return;
        var currentPieceModel = CurrentPiece.GetComponent<Piece>();
        if (currentPieceModel.Letter == "O" || currentPieceModel.Letter == "D")
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
                if (!IsNextGravityFallPossible())
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

    public void AntiClock()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked || CurrentPiece.GetComponent<Piece>().IsMimic)
            return;
        var currentPieceModel = CurrentPiece.GetComponent<Piece>();
        if (currentPieceModel.Letter == "O" || currentPieceModel.Letter == "D")
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
                if (!IsNextGravityFallPossible())
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

    public void Hold()
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
            CurrentGhost.GetComponent<Piece>().SetColor(_ghostColor);
            if (_currentGhostPiecesOriginalPos != null)
                _currentGhostPiecesOriginalPos.Clear();
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

    public void Item()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        if (_characterItem != null)
        {
            
            if (_characterItem.Activate(Character, this))
                _soundControler.PlaySound(_idItem);
            UpdateItemAndSpecialVisuals();
        }
    }

    public void Special()
    {
        if (CurrentPiece.GetComponent<Piece>().IsLocked)
            return;
        if (_characterSpecial.Activate())
            _soundControler.PlaySound(_idSpecial);
        UpdateItemAndSpecialVisuals();
    }

    private bool IsPiecePosValidOrReset(bool isGravity = false, bool mimicPossible = false)
    {
        if (IsPiecePosValid(CurrentPiece, mimicPossible) == false)
        {
            CurrentPiece.transform.position = _lastCurrentPieceValidPosition;
            if (Character.CanMimic)
                CancelMimicPiece(CurrentPiece.transform);
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


    private bool IsPiecePosValid(GameObject piece, bool mimicPossible = false)
    {
        int id = 0;
        foreach (Transform child in piece.transform)
        {
            if (!IsBlockPosValid(child, id, piece.transform, mimicPossible))
                return false;
            ++id;
        }
        return true;
    }

    private bool IsBlockPosValid(Transform block, int blockId, Transform piece, bool mimicPossible = false)
    {
        int roundedX = Mathf.RoundToInt(block.transform.position.x);
        int roundedY = Mathf.RoundToInt(block.transform.position.y);

        if (roundedX < 0 || roundedX >= _playFieldWidth)
        {
            if (mimicPossible && Character.CanMimic && piece.GetComponent<Piece>().GetNbBlocksMimicked() < piece.transform.childCount)
                roundedX = MimicBlock(block, blockId, piece);
            else
                return false;
        }
        if (roundedX == -1)
            return false;

        if (roundedY < 0 || roundedY >= _playFieldHeight)
            return false;

        if (PlayFieldBhv.Grid[roundedX, roundedY] != null)
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

    private int MimicBlock(Transform block, int blockId, Transform piece)
    {
        var lastBlockPos = block.position;
        if (block.transform.position.x < 0)
            block.position += new Vector3(10.0f, 0.0f, 0.0f);
        else if (block.transform.position.x > 9)
            block.position += new Vector3(-10.0f, 0.0f, 0.0f);

        if (!IsBlockPosValid(block, blockId, piece))
        {
            block.position = lastBlockPos;
            return -1;
        }
        if (Vector2.Distance(new Vector2(block.position.x, 0.0f), new Vector2(piece.position.x, 0.0f)) > 4.0f)
        {
            if (block.GetComponent<BlockBhv>() != null)
                block.GetComponent<BlockBhv>().SetMimicAppearance();
            else
                block.GetComponent<SpriteRenderer>().color = (Color)Constants.GetColorFromNature(Helper.GetInferiorFrom(Character.Realm), 4);
        }
        else
        {
            if (block.GetComponent<BlockBhv>() != null)
            {
                block.GetComponent<BlockBhv>().UnsetMimicAppearance();
                if (CurrentGhost != null)
                    CurrentGhost.transform.GetChild(blockId).GetComponent<SpriteRenderer>().color = _ghostColor;
            }
        }
        piece.GetComponent<Piece>().CheckAndSetMimicStatus();
        return Mathf.RoundToInt(block.transform.position.x);
    }

    private void CancelMimicPiece(Transform piece)
    {
        int id = 0;
        foreach (Transform block in piece)
        {
            MimicBlock(block, id, piece);
            ++id;
        }
    }

    private bool IsPiecesBlocksOverlappingGhost(GameObject piece)
    {
        if (CurrentGhost == null)
            return false;
        foreach (Transform child in piece.transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);

            foreach (Transform ghostChild in CurrentGhost.transform)
            {
                int ghostChildRoundedX = Mathf.RoundToInt(ghostChild.transform.position.x);
                int ghostChildRoundedY = Mathf.RoundToInt(ghostChild.transform.position.y);
                if (roundedX == ghostChildRoundedX && roundedY == ghostChildRoundedY)
                    return true;
            }
        }
        return false;
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
        var canSpawn = true;
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

            if (nbLines >= 2)
                CheckForDarkRows(nbLines - 1);

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
                SceneBhv.OnCombo(_comboCounter, nbLines);
            }

            if (GetHighestBlock() == -1) //PERFECT
            {
                _soundControler.PlaySound(_idPerfect);
                _characterSpecial.OnPerfectClear();
                SceneBhv.OnPerfectClear();
            }                

            SceneBhv.PopText();
            UpdateItemAndSpecialVisuals();
            StartCoroutine(Helper.ExecuteAfterDelay(0.3f, () =>
            {
                ClearLineSpace();
                if (AttackIncoming)
                {
                    AttackIncoming = false;
                    //Debug.Log(DateTime.Now + "CheckForLine (line)");
                    canSpawn = SceneBhv.OpponentAttack();
                }
                if (canSpawn)
                    Spawn();
                return true;
            }));
            
        }
        else
        {
            SceneBhv.OnLinesCleared(nbLines, false);
            SceneBhv.PopText();
            _comboCounter = 0;
            if (AttackIncoming)
            {
                AttackIncoming = false;
                //Debug.Log(DateTime.Now + "CheckForLine (no line)");
                canSpawn = SceneBhv.OpponentAttack();
            }
            if (canSpawn)
                Spawn();
        }            
    }

    public void CheckForDarkRows(int nbLines)
    {
        bool hasDeletedRows = false;
        for (int y = _playFieldHeight - 1; y >= 0; --y)
        {
            if (HasDarkRow(y))
            {
                if (hasDeletedRows == false)
                    _soundControler.PlaySound(_idCleanRows);
                hasDeletedRows = true;
                DeleteLine(y);
                --nbLines;
            }
            if (nbLines <= 0)
                return;
        }
    }

    public void CheckForWasteRows(int nbLines)
    {
        bool hasDeletedRows = false;
        for (int y = _playFieldHeight - 1; y >= 0; --y)
        {
            if (HasWasteRow(y))
            {
                if (hasDeletedRows == false)
                    _soundControler.PlaySound(_idCleanRows);
                hasDeletedRows = true;
                DeleteLine(y);
                --nbLines;
            }
            if (nbLines <= 0)
                return;
        }
    }

    private void CheckForLightRows()
    {
        int startY;
        int nbRows;
        bool hasDeletedRows = false;
        var allLightRows = GameObject.FindGameObjectsWithTag(Constants.TagLightRows);
        foreach (var lightRowGameObject in allLightRows)
        {
            var lightRowBhv = lightRowGameObject.GetComponent<LightRowBlockBhv>();
            if (lightRowBhv.IsOverOrDecreaseCooldown())
            {
                int yRounded = Mathf.RoundToInt(lightRowGameObject.transform.position.y);
                startY = yRounded;
                nbRows = lightRowBhv.NbRows;
                for (int y = yRounded; y < yRounded + lightRowBhv.NbRows; ++y)
                {
                    if (hasDeletedRows == false)
                        _soundControler.PlaySound(_idCleanRows);
                    hasDeletedRows = true;
                    DeleteLine(y);
                }
                ClearLineSpace(startY, startY + nbRows - 1);
            }
        }
    }

    private void CheckForVisionBlocks()
    {
        var allVisionBlocks = GameObject.FindGameObjectsWithTag(Constants.TagVisionBlock);
        foreach (var visionBlockGameObject in allVisionBlocks)
        {
            var visionBlockBhv = visionBlockGameObject.GetComponent<VisionBlockBhv>();
            if (visionBlockBhv != null)
            {
                visionBlockBhv.DecreaseCooldown(Character.VisionBlockReducer, pop: true);
            }
        }
    }

    private bool HasLine(int y)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            if (PlayFieldBhv.Grid[x, y] == null || PlayFieldBhv.Grid[x, y].gameObject.name.Contains("Dark") || PlayFieldBhv.Grid[x, y].gameObject.name.Contains("Light"))
                return false;
        }
        return true;
    }

    private bool HasDarkRow(int y)
    {
        return PlayFieldBhv.Grid[0, y] != null && PlayFieldBhv.Grid[0, y].gameObject.name.Contains("Dark");
    }

    private bool HasWasteRow(int y)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            if (PlayFieldBhv.Grid[x, y] != null && PlayFieldBhv.Grid[x, y].gameObject.name.Contains("Waste"))
                return true;
        }
        return false;
    }

    public void DeleteLine(int y)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            if (PlayFieldBhv.Grid[x, y] == null)
                continue;
            Instantiator.NewFadeBlock(_characterRealm, PlayFieldBhv.Grid[x, y].transform.position, 5, 0);
            Destroy(PlayFieldBhv.Grid[x, y].gameObject);
            PlayFieldBhv.Grid[x, y] = null;
        }
    }

    public void ClearLineSpace(int minY = -1, int maxY = -1)
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
                if (maxY != -1 && minY != -1
                    && (y < minY || y > maxY))
                    continue;
                DropAllAboveLines(y);
                y = -1;
                if (maxY != -1 && minY != -1 && --maxY < minY)
                    maxY = minY = -2;
            }
        }
        foreach (Transform child in PlayFieldBhv.transform)
        {
            if (child.childCount == 0 && child.GetComponent<Piece>() != null)
                Destroy(child.gameObject);
        }
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
            if (PlayFieldBhv.Grid[x, y] != null)
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
                if (PlayFieldBhv.Grid[x, y] != null)
                {
                    PlayFieldBhv.Grid[x, y - 1] = PlayFieldBhv.Grid[x, y];
                    PlayFieldBhv.Grid[x, y] = null;
                    PlayFieldBhv.Grid[x, y - 1].transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                }
            }
        }
    }

    private void IncreaseAllAboveLines(int nbRows)
    {
        for (int y = _playFieldHeight - 1; y >= 0; --y)
        {
            for (int x = 0; x < _playFieldWidth; ++x)
            {
                if (PlayFieldBhv.Grid[x, y] != null)
                {
                    PlayFieldBhv.Grid[x, y + nbRows] = PlayFieldBhv.Grid[x, y];
                    PlayFieldBhv.Grid[x, y] = null;
                    PlayFieldBhv.Grid[x, y + nbRows].transform.position += new Vector3(0.0f, nbRows, 0.0f);
                }
            }
        }
    }

    public void OpponentAttack(AttackType type, int param1, int param2, Realm opponentRealm, GameObject opponentInstance)
    {
        VibrationService.Vibrate();
        switch (type)
        {
            case AttackType.DarkRow:
                AttackDarkRows(opponentInstance, param1, opponentRealm);
                break;
            case AttackType.WasteRow:
                AttackGarbageRows(opponentInstance, param1, opponentRealm, param2);
                break;
            case AttackType.LightRow:
                AttackLightRows(opponentInstance, param1, opponentRealm, param2);
                break;
            case AttackType.EmptyRow:
                AttackEmptyRows(opponentInstance, param1, opponentRealm);
                break;
            case AttackType.VisionBlock:
                AttackVisionBlock(opponentInstance, param1, opponentRealm, param2);
                break;
            case AttackType.ForcedPiece:
                AttackForcedPiece(opponentInstance, opponentRealm, param1, param2);
                break;
        }
        if (_characterItem != null
            && (type == AttackType.DarkRow
            || type == AttackType.WasteRow
            || type == AttackType.LightRow
            || type == AttackType.EmptyRow))
            Constants.CurrentItemCooldown -= Character.ItemCooldownReducer * param1;
        else if (_characterItem != null
            && (type == AttackType.VisionBlock
            || type == AttackType.ForcedPiece))
            Constants.CurrentItemCooldown -= Character.ItemCooldownReducer;
        UpdateItemAndSpecialVisuals();
    }

    private void AttackDarkRows(GameObject opponentInstance, int nbRows, Realm opponentRealm)
    {
        IncreaseAllAboveLines(nbRows);
        _soundControler.PlaySound(_idDarkRows);
        for (int y = 0; y < nbRows; ++y)
        {
            FillLine(y, AttackType.DarkRow, opponentRealm);
        }
        Instantiator.NewAttackLine(opponentInstance.transform.position, new Vector3(4.5f, (float)nbRows / 2.0f - 0.5f, 0.0f), Character.Realm);
    }

    private void AttackGarbageRows(GameObject opponentInstance, int nbRows, Realm opponentRealm, int param)
    {
        IncreaseAllAboveLines(nbRows);
        _soundControler.PlaySound(_idGarbageRows);
        param = param < 1 ? 1 : param;
        int emptyStart = UnityEngine.Random.Range(0, 10 + 1 - param);
        int emptyEnd = emptyStart + param - 1;
        for (int y = 0; y < nbRows; ++y)
        {
            FillLine(y, AttackType.WasteRow, opponentRealm, emptyStart, emptyEnd);
        }
        Instantiator.NewAttackLine(opponentInstance.transform.position, new Vector3(((emptyEnd - emptyStart) / 2) + emptyStart, (float)nbRows / 2.0f - 0.5f, 0.0f), Character.Realm);
    }

    private void AttackLightRows(GameObject opponentInstance, int nbRows, Realm opponentRealm, int param)
    {
        IncreaseAllAboveLines(nbRows);
        _soundControler.PlaySound(_idLightRows);
        for (int y = 0; y < nbRows; ++y)
        {
            FillLine(y, AttackType.LightRow, opponentRealm);
        }
        param = param < 1 ? 1 : param;
        PlayFieldBhv.Grid[0, 0].gameObject.tag = Constants.TagLightRows;
        PlayFieldBhv.Grid[0, 0].gameObject.AddComponent<LightRowBlockBhv>();
        var lightRowBhv = PlayFieldBhv.Grid[0, 0].gameObject.GetComponent<LightRowBlockBhv>();
        lightRowBhv.NbRows = nbRows;
        lightRowBhv.Cooldown = param;
        var tmpTextGameObject = Instantiator.NewLightRowText(new Vector2(4.5f, ((float)nbRows - 1.0f) / 2.0f));
        tmpTextGameObject.transform.SetParent(PlayFieldBhv.Grid[0, 0]);
        lightRowBhv.CooldownText = tmpTextGameObject.GetComponent<TMPro.TextMeshPro>();
        lightRowBhv.UpdateCooldownText(param);
        Instantiator.NewAttackLine(opponentInstance.transform.position, new Vector3(4.5f, (float)nbRows / 2.0f - 0.5f, 0.0f), Character.Realm);
    }

    private void AttackEmptyRows(GameObject opponentInstance, int nbRows, Realm opponentRealm)
    {
        IncreaseAllAboveLines(nbRows);
        _soundControler.PlaySound(_idEmptyRows);
        Instantiator.NewAttackLine(opponentInstance.transform.position, new Vector3(4.5f, (float)nbRows / 2.0f - 0.5f, 0.0f), Character.Realm);
    }

    private void AttackVisionBlock(GameObject opponentInstance, int nbRows, Realm opponentRealm, int param)
    {
        _soundControler.PlaySound(_idVisionBlock);
        nbRows = nbRows < 2 ? 2 : nbRows;
        var currentHiest = GetHighestBlock();
        if (currentHiest + nbRows > 19)
            currentHiest = 19 - nbRows;
        var visionBlockInstance = Instantiator.NewVisionBlock(new Vector2(4.5f, (((float)nbRows - 1.0f) / 2.0f) + (float)currentHiest), nbRows, param, opponentRealm);
        visionBlockInstance.transform.SetParent(PlayFieldBhv.gameObject.transform);
        Instantiator.NewAttackLine(opponentInstance.transform.position, visionBlockInstance.transform.position, Character.Realm);
    }

    public void AttackForcedPiece(GameObject opponentInstance, Realm opponentRealm, int letter, int rotation)
    {
        if (ForcedPiece != null)
        {
            if (_forcedPieceModel == null)
                SetForcedPieceModel();
            CurrentPiece = Instantiator.NewPiece(_forcedPieceModel.Letter, opponentRealm.ToString(), new Vector3(50.0f, 50.0f, 0.0f));
            CurrentPiece.transform.position = new Vector3(ForcedPiece.transform.position.x, _spawner.transform.position.y + 10.0f + _forcedPieceModel.YFromSpawn, 0.0f);
            CurrentPiece.transform.rotation = ForcedPiece.transform.rotation;
            CurrentPiece.name = "Old" + Constants.GoForcedPiece;
            Destroy(ForcedPiece);
            CurrentPiece.GetComponent<Piece>().IsLocked = true;
            Instantiator.NewAttackLine(opponentInstance.transform.position, CurrentPiece.transform.position, Character.Realm);
            Invoke(nameof(ForcedPieceHardDrop), 0.25f);
        }
        else
        {
            var alreadyExistingForcedPiece = GameObject.Find(Constants.GoForcedPiece);
            if (alreadyExistingForcedPiece != null)
            {
                ForcedPiece = alreadyExistingForcedPiece;
                return;
            }
            var numberRotation = rotation == -1 ? UnityEngine.Random.Range(0, 4) : rotation;
            var randomX = UnityEngine.Random.Range(-4, 6);
            var forcedPieceLetter = Constants.PiecesLetters[letter < 0 ? UnityEngine.Random.Range(0, Constants.PiecesLetters.Length) : letter].ToString();
            if (letter == -2)
                forcedPieceLetter = "D";
            ForcedPiece = Instantiator.NewPiece(forcedPieceLetter, opponentRealm.ToString() + "Ghost", _spawner.transform.position + new Vector3(0.0f, 10.0f, 0.0f));
            ForcedPiece.name = Constants.GoForcedPiece;
            SetForcedPieceModel();
            //var tmpFadeBlockSpriteRenderer = Instantiator.NewFadeBlock(opponentRealm, new Vector3(50.0f, 50.0f, 1.0f), 5, 5).GetComponent<SpriteRenderer>();
            //foreach (Transform child in forcedPiece.transform)
            //{
            //    var childSpriteRenderer = child.GetComponent<SpriteRenderer>();
            //    childSpriteRenderer.sprite = tmpFadeBlockSpriteRenderer.sprite;
            //    childSpriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            //}
            var color = int.Parse(PlayerPrefsHelper.GetGhostColor()) == 3 ? 4 : 3;
            _forcedPieceModel.SetColor((Color)Constants.GetColorFromNature(_characterRealm, color));
            //Destroy(tmpFadeBlockSpriteRenderer.gameObject);
            if (_forcedPieceModel.Letter != "O")
            {
                for (int i = 0; i < numberRotation; ++i)
                    ForcedPiece.transform.Rotate(0.0f, 0.0f, -90.0f);
            }
            for (int j = 0; ((int)ForcedPiece.transform.position.x) != (4 + randomX) || j > 10; ++j)
            {
                var lastPos = ForcedPiece.transform.position;
                ForcedPiece.transform.position += new Vector3(1.0f * (randomX < 0 ? -1.0f : 1.0f), 0.0f, 0.0f);
                if (IsPiecePosValid(ForcedPiece) == false)
                {
                    ForcedPiece.transform.position = lastPos;
                    break;
                }
            }
            ForcedPiece.transform.SetParent(PlayFieldBhv.gameObject.transform);
            DropForcedPiece();
        }
    }

    public void SetForcedPieceOpacity(float current, float max)
    {
        if (_forcedPieceModel == null)
        {
            if (ForcedPiece == null)
                return;
            SetForcedPieceModel();
        }
        _forcedPieceModel.HandleOpacityOnLock((current / max) + 0.2f);
    }

    private void SetForcedPieceModel()
    {
        _forcedPieceModel = ForcedPiece.GetComponent<Piece>();
    }

    private void FillLine(int y, AttackType type, Realm realm, int emptyStart = -1, int emptyEnd = -1)
    {
        for (int x = 0; x < _playFieldWidth; ++x)
        {
            if (type == AttackType.WasteRow && x >= emptyStart && x <= emptyEnd)
                continue;
            var attackBlock = Instantiator.NewPiece(type.ToString(), realm.ToString(), new Vector3(x, y, 0.0f));
            attackBlock.transform.SetParent(PlayFieldBhv.gameObject.transform);
            PlayFieldBhv.Grid[x, y] = attackBlock.transform;
        }
    }

    #endregion
}
