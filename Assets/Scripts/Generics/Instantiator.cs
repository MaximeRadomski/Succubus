﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static AccountSceneBhv;

public class Instantiator : MonoBehaviour
{
    private Camera _mainCamera;
    private bool _isClassic;
    private bool _hasInit;

    void Start()
    {
        if (!_hasInit)
            Init();
    }

    public void Init()
    {
        _mainCamera = Helper.GetMainCamera();
        _isClassic = PlayerPrefsHelper.GetClassicPieces() || Cache.CurrentGameMode == GameMode.TrainingOldSchool;
        _hasInit = true;
    }

    public GameObject NewLevel(float margin, int realm, int layer, GameObject parent)
    {
        var tmpLevelObject = Resources.Load<GameObject>("Prefabs/Level");
        var tmpLevelInstance = Instantiate(tmpLevelObject, new Vector3(parent.transform.position.x, parent.transform.position.y + margin, 0.0f), tmpLevelObject.transform.rotation);
        tmpLevelInstance.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Levels_" + realm);
        tmpLevelInstance.GetComponent<SpriteRenderer>().sortingOrder = layer;
        tmpLevelInstance.transform.SetParent(parent.transform);
        return tmpLevelInstance;
    }

    public void New321(Vector3 position, Action afterAnimation)
    {
        var tmp321Object = Resources.Load<GameObject>("Prefabs/321");
        var tmp321Instance = Instantiate(tmp321Object, position, tmp321Object.transform.rotation);
        tmp321Instance.GetComponent<FramesAnimationBhv>().Init(afterAnimation);
    }

    public void NewBackgroundPiece(Vector3 position, Action afterAnimation)
    {
        var tmpBackgroundPieceObject = Resources.Load<GameObject>("Prefabs/BackgroundPiece");
        var tmpBackgroundPieceInstance = Instantiate(tmpBackgroundPieceObject, position, tmpBackgroundPieceObject.transform.rotation);
        DontDestroyOnLoad(tmpBackgroundPieceInstance);
        tmpBackgroundPieceInstance.GetComponent<FramesAnimationBhv>().Init(afterAnimation);
    }
    public void NewBackgroundLine(Vector3 position, Action afterAnimation)
    {
        var tmpBackgroundLineObject = Resources.Load<GameObject>("Prefabs/BackgroundLine");
        var tmpBackgroundLineInstance = Instantiate(tmpBackgroundLineObject, position, tmpBackgroundLineObject.transform.rotation);
        DontDestroyOnLoad(tmpBackgroundLineInstance);
        tmpBackgroundLineInstance.GetComponent<FramesAnimationBhv>().Init(afterAnimation);
    }

    public void NewBackgroundCross(Vector3 position, Action afterAnimation)
    {
        var tmpBackgroundCrossObject = Resources.Load<GameObject>("Prefabs/BackgroundCross");
        var tmpBackgroundCrossInstance = Instantiate(tmpBackgroundCrossObject, position, tmpBackgroundCrossObject.transform.rotation);
        DontDestroyOnLoad(tmpBackgroundCrossInstance);
        tmpBackgroundCrossInstance.GetComponent<FramesAnimationBhv>().Init(afterAnimation);
    }

    public GameObject NewRotationPoint(GameObject piece)
    {
        var tmpRotationPointObject = Resources.Load<GameObject>("Prefabs/RotationPoint");
        var tmpRotationPointInstance = Instantiate(tmpRotationPointObject, piece.transform.position, tmpRotationPointObject.transform.rotation);
        tmpRotationPointInstance.name = Constants.GoRotationPoint;
        tmpRotationPointInstance.transform.SetParent(piece.transform.GetChild(0));
        return tmpRotationPointInstance;
    }

    public GameObject NewScoreHistory(int score, int max, int id, GameObject parent)
    {
        var height = 8 * Constants.Pixel;
        var tmpScoreHystoryObject = Resources.Load<GameObject>("Prefabs/ScoreHistory");
        var tmpScoreHystoryInstance = Instantiate(tmpScoreHystoryObject, new Vector3(0.0f, parent.transform.position.y - (id * height), 0.0f), tmpScoreHystoryObject.transform.rotation);
        tmpScoreHystoryInstance.transform.Find("Score").GetComponent<TMPro.TextMeshPro>().text = score.ToString();
        var bar = tmpScoreHystoryInstance.transform.Find("Bar").GetComponent<ResourceBarBhv>();
        bar.UpdateContent(0, max);
        bar.UpdateContent(score, max, Direction.Up);
        tmpScoreHystoryInstance.transform.SetParent(parent.transform);
        return tmpScoreHystoryInstance;
    }

    public GameObject NewAttackLine(Vector3 source, Vector3 target, Realm realm, bool linear = true, Sprite sprite = null, Action onPop = null)
    {
        var attackLine = new GameObject("AttackLine");
        attackLine.AddComponent<AttackLineBhv>();
        attackLine.GetComponent<AttackLineBhv>().Init(source, target, realm, this, linear, sprite, onPop);
        return attackLine;
    }

    public GameObject NewStepsContainer()
    {
        var tmpStepsContainerInstance = Instantiate(new GameObject(), new Vector3(0.0f, 0.0f, 0.0f), new Quaternion());
        tmpStepsContainerInstance.name = "StepsContainer";
        return tmpStepsContainerInstance;
    }

    public GameObject NewStepInstance(Step step, GameObject mask, Run run, bool mapAquired)
    {
        var tmpStepObject = Resources.Load<GameObject>("Prefabs/Step");
        var tmpStepInstance = Instantiate(tmpStepObject, Helper.TransformFromStepCoordinates(step.X, step.Y), tmpStepObject.transform.rotation);
        tmpStepInstance.GetComponent<StepInstanceBhv>().UpdateVisual(step, run, mapAquired);
        tmpStepInstance.transform.name = step.X + "_" + step.Y;
        tmpStepInstance.GetComponent<MaskLinkerBhv>().Mask = mask;
        return tmpStepInstance;
    }

    public GameObject NewDrillTarget(Realm realm, Vector3 position)
    {
        var tmpDrillTargetObject = Resources.Load<GameObject>("Prefabs/DrillTarget");
        var tmpDrillTargetInstance = Instantiate(tmpDrillTargetObject, position, tmpDrillTargetObject.transform.rotation);
        tmpDrillTargetInstance.GetComponent<SpriteRenderer>().color = (Color)Constants.GetColorFromRealm(realm, 4);
        tmpDrillTargetInstance.name = Constants.GoDrillTarget;
        return tmpDrillTargetInstance;
    }

    public GameObject NewDrone(Realm realm, Vector3 position, GameplayControler gameControler, int nbRows, AttackType rowType)
    {
        var tmpDroneTargetObject = Resources.Load<GameObject>("Prefabs/Drone");
        var tmpDroneTargetInstance = Instantiate(tmpDroneTargetObject, position, tmpDroneTargetObject.transform.rotation);
        tmpDroneTargetInstance.GetComponent<DroneBhv>().Init(gameControler, realm, nbRows, rowType);
        //tmpDroneTargetInstance.GetComponent<SpriteRenderer>().color = (Color)Constants.GetColorFromRealm(realm, 4);
        tmpDroneTargetInstance.name = Constants.GoDrone;
        return tmpDroneTargetInstance;
    }

    public GameObject NewPiece(string pieceLetter, string realm, Vector3 spawnerPosition, bool keepSpawnerX = false)
    {
        if (_isClassic || realm == Realm.None.ToString())
        {
            var ghost = realm.Contains("Ghost") ? "Ghost" : "";
            realm = "Classic" + ghost;
        }
        var tmpPieceObject = Resources.Load<GameObject>("Prefabs/" + pieceLetter + "-" + realm);
        var pieceModel = tmpPieceObject.GetComponent<Piece>();
        if (pieceModel != null)
            return Instantiate(tmpPieceObject, spawnerPosition + new Vector3(keepSpawnerX ? 0.0f : pieceModel.XFromSpawn, pieceModel.YFromSpawn, 0.0f), tmpPieceObject.transform.rotation); 
        else
            return Instantiate(tmpPieceObject, spawnerPosition, tmpPieceObject.transform.rotation);
    }

    public GameObject NewPieceBlock(string realm, Vector3 spawnPosition, Transform parent)
    {
        var tmpPieceObject = Resources.Load<GameObject>("Prefabs/D-" + realm);
        var tmpPieceBlockObject = tmpPieceObject.transform.GetChild(0);
        GameObject tmpPieceBlockInstance = Instantiate(tmpPieceBlockObject.gameObject, spawnPosition, tmpPieceBlockObject.rotation);
        tmpPieceBlockInstance.transform.SetParent(parent);
        tmpPieceBlockInstance.name = "Forced";
        return tmpPieceBlockInstance;
    }

    public GameObject NewBlockLockEffect(int[] minMaxX, int minY)
    {
        var tmpParticlesObject = Resources.Load<GameObject>("Prefabs/BlockLockParticles");
        float width = (minMaxX[1] - minMaxX[0]) + 1;
        float half = width / 2.0f;
        GameObject tmpParticlesInstance = Instantiate(tmpParticlesObject, new Vector3(((float)minMaxX[0] - 0.5f) + half, minY - 0.5f, 0.0f), tmpParticlesObject.transform.rotation);
        var shadow = tmpParticlesInstance.transform.GetChild(0);
        shadow.localScale = new Vector3(shadow.localScale.x * width, shadow.localScale.y, shadow.localScale.z);
        return tmpParticlesInstance;
    }

    public void NewGravitySquare(GameObject parent, string realm)
    {
        var realmStr = "" + realm;
        if (_isClassic)
            realmStr = "Classic";
        var tmpSquareObject = Resources.Load<GameObject>("Prefabs/GravitySquare-" + realmStr);
        var tmpSquareInstance = Instantiate(tmpSquareObject, parent.transform.position, tmpSquareObject.transform.rotation);
        tmpSquareInstance.transform.SetParent(parent.transform);
    }

    public GameObject NewFadeBlock(Realm realm, Vector3 position, int startColor, int endColor)
    {
        var realmStr = "" + realm;
        if (_isClassic)
            realmStr = "Classic";
        var tmpBlockObject = Resources.Load<GameObject>("Prefabs/FadeBlock" + realmStr);
        var tmpBlockInstance = Instantiate(tmpBlockObject, position, tmpBlockObject.transform.rotation);
        tmpBlockInstance.GetComponent<FadeBlockBhv>().Init(startColor, endColor, realm);
        return tmpBlockInstance;
    }

    public GameObject NewReflectBlock(string name, Vector3 position, float speed = 0.05f, Color? color = null)
    {
        var tmpBlockObject = Resources.Load<GameObject>("Prefabs/" + name);
        if (tmpBlockObject == null)
            return null;
        var tmpBlockInstance = Instantiate(tmpBlockObject, position, tmpBlockObject.transform.rotation);
        tmpBlockInstance.GetComponent<FadeOnAppearanceBhv>().Init(speed, color);
        return tmpBlockInstance;
    }

    public void EditViaKeyboard(GameObject target, Identifier identifier, bool isPassword = false)
    {
        ShowKeyboard(target.GetComponent<TMPro.TextMeshPro>(), identifier, isPassword, target.GetComponent<BoxCollider2D>().size.x);
    }

    public void ShowKeyboard(TMPro.TextMeshPro target, Identifier identifier, bool isPassword, float maxWidth = -1)
    {
        if (!_hasInit)
            Init();
        var tmpKeyboardObject = Resources.Load<GameObject>("Prefabs/Keyboard");
        var tmpKeyboardInstance = Instantiate(tmpKeyboardObject, tmpKeyboardObject.transform.position, tmpKeyboardObject.transform.rotation);
        tmpKeyboardInstance.name = Constants.GoKeyboard;
        Cache.IncreaseInputLayer(tmpKeyboardInstance.name);
        Cache.KeyboardUp = true;
        for (int i = 0; i < tmpKeyboardInstance.transform.childCount; ++i)
        {
            var inputKeyBhv = tmpKeyboardInstance.transform.GetChild(i).GetComponent<InputKeyBhv>();
            if (inputKeyBhv != null)
                inputKeyBhv.SetPrivates(target, identifier, isPassword, maxWidth);
        }
        tmpKeyboardInstance.transform.Find("InputKeyLayout" + PlayerPrefs.GetInt(Constants.PpFavKeyboardLayout, Constants.PpFavKeyboardLayoutDefault)).GetComponent<InputKeyBhv>().ChangeLayout();
        if (target.transform.position.y < -_mainCamera.orthographicSize + Constants.KeyboardHeight)
            _mainCamera.gameObject.GetComponent<CameraBhv>().FocusY(target.transform.position.y + (_mainCamera.orthographicSize - Constants.KeyboardHeight));
    }

    public GameObject NewPopupYesNo(string title, string content, string negative, string positive,
        Action<bool> resultAction, Sprite sprite = null, bool defaultPositive = false, bool big = false)
    {
        if (!_hasInit)
            Init();
        var tmpPopupObject = Resources.Load<GameObject>($"Prefabs/PopupYesNo{(big ? "Big" : string.Empty)}");
        var tmpPopupInstance = Instantiate(tmpPopupObject, new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0.0f), tmpPopupObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupYesNoBhv>().Init(title, content, negative, positive, resultAction, sprite, defaultPositive);
        tmpPopupInstance.transform.SetParent(this._mainCamera.transform);
        return tmpPopupInstance;
    }

    public void NewPopupCharacterSkins(int charId, List<int> unlockedSkins, int currentSelectedSkin, System.Action<int> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupCharacterSkins");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupCharacterSkinsBhv>().Init(charId, unlockedSkins, currentSelectedSkin, resultAction);
    }

    public void NewPopupTrainingFreeLevel(System.Action<int> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupTrainingFreeLevel");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupTrainingFreeLevelBhv>().Init(resultAction);
    }

    public void NewPopupGameplayButtons(Action<bool> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupGameplayButtons");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupGameplayButtonsBhv>().Init(resultAction, this);
    }

    public GameObject NewToast(string content, float duration = 1.5f)
    {
        if (!_hasInit)
            Init();
        var tmpToastObject = Resources.Load<GameObject>($"Prefabs/Toast");
        var tmpToastInstance = Instantiate(tmpToastObject, new Vector3(_mainCamera.transform.position.x - 30.0f, _mainCamera.transform.position.y - 30.0f, 0.0f), tmpToastObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpToastInstance.name);
        tmpToastInstance.GetComponent<ToastBhv>().Init(content, duration);
        tmpToastInstance.transform.SetParent(this._mainCamera.transform);
        return tmpToastInstance;
    }

    public GameObject NewGameplayButton(string name, Vector3 position)
    {
        var tmpButtonObject = Resources.Load<GameObject>("Prefabs/" + name);
        var tmpButtonInstance = Instantiate(tmpButtonObject, position, tmpButtonObject.transform.rotation);
        tmpButtonInstance.name = tmpButtonInstance.name.Replace("(Clone)", "");
        return tmpButtonInstance;
    }

    public void NewOverBlend(OverBlendType overBlendType, string message, float? constantLoadingSpeed,
        Func<bool, bool> resultAction, bool reverse = false)
    {
        var already = GameObject.FindGameObjectsWithTag(Constants.TagOverBlend);
        if (already != null && already.Length > 0 && already[0].GetComponent<OverBlendBhv>().State < 2 && !already[0].GetComponent<OverBlendBhv>().HasResulted)
            return;
        var tmpOverBlendObject = Resources.Load<GameObject>("Prefabs/OverBlend");
        var tmpOverBlendInstance = Instantiate(tmpOverBlendObject, tmpOverBlendObject.transform.position, tmpOverBlendObject.transform.rotation);
        tmpOverBlendInstance.GetComponent<OverBlendBhv>().Init(overBlendType, message, constantLoadingSpeed, resultAction, reverse);
    }

    public void NewSnack(string content, float duration = 2.0f)
    {
        var tmpSnackObject = Resources.Load<GameObject>("Prefabs/Snack");
        var tmpSnackInstance = Instantiate(tmpSnackObject, tmpSnackObject.transform.position, tmpSnackObject.transform.rotation);
        tmpSnackInstance.GetComponent<SnackBhv>().SetPrivates(content, duration);
    }

    public GameObject NewPauseMenu(Action<bool> resumeAction, GameSceneBhv callingScene = null, bool isHorizontal = false)
    {
        var tmpPauseMenuObject = Resources.Load<GameObject>("Prefabs/PauseMenu");
        var tmpPauseMeuInstance = Instantiate(tmpPauseMenuObject, tmpPauseMenuObject.transform.position, tmpPauseMenuObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpPauseMeuInstance.name);
        tmpPauseMeuInstance.GetComponent<PauseMenuBhv>().Init(this, resumeAction, callingScene, isHorizontal);
        return tmpPauseMeuInstance;
    }

    public GameObject NewMenuSelector()
    {
        var tmpMenuSelectorObject = Resources.Load<GameObject>("Prefabs/MenuSelector");
        var tmpMenuSelectorInstance = Instantiate(tmpMenuSelectorObject, tmpMenuSelectorObject.transform.position, tmpMenuSelectorObject.transform.rotation);
        return tmpMenuSelectorInstance;
    }

    public GameObject NewInfoMenu(Action<bool> resumeAction, Character character, Opponent opponent, bool isHorizontal = false)
    {
        if (Cache.CurrentGameMode == GameMode.TrainingOldSchool)
        {
            NewPopupYesNo("Unavailable", "info menu is unavailable in old school mode", null, "Ok", null);
            return null;
        }
        var tmpInfoMenuObject = Resources.Load<GameObject>("Prefabs/InfoMenu");
        var tmpInfoMeuInstance = Instantiate(tmpInfoMenuObject, tmpInfoMenuObject.transform.position, tmpInfoMenuObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpInfoMeuInstance.name);
        tmpInfoMeuInstance.GetComponent<InfoMenuBhv>().Init(this, resumeAction, character, opponent, isHorizontal);
        return tmpInfoMeuInstance;
    }

    public GameObject PopText(string text, Vector2 position, float floatingTime = 0.0f, float speed = 0.05f, float distance = 0.25f, float startFadingDistancePercent = 0.04f, float fadingSpeed = 0.1f)
    {
        var tmpPoppingTextObject = Resources.Load<GameObject>("Prefabs/PoppingText");
        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().Init(text, position, floatingTime, speed, distance, startFadingDistancePercent, fadingSpeed);
        return tmpPoppingTextInstance;
    }

    public GameObject NewLightRowText(Vector2 position)
    {
        var tmpTextObject = Resources.Load<GameObject>("Prefabs/LightRowText");
        var tmpTextInstance = Instantiate(tmpTextObject, position, tmpTextObject.transform.rotation);
        return tmpTextInstance;
    }

    public GameObject NewVisionBlock(Vector2 position, int nbRows, int nbSeconds, Realm opponentRealm, GameplayControler gameplayControler)
    {
        var tmpVisionBlockObject = Resources.Load<GameObject>("Prefabs/VisionBlock");
        var tmpVisionBlockInstance = Instantiate(tmpVisionBlockObject, position, tmpVisionBlockObject.transform.rotation);
        tmpVisionBlockInstance.GetComponent<VisionBlockBhv>().Init(nbRows, nbSeconds, opponentRealm, gameplayControler);
        return tmpVisionBlockInstance;
    }

    public GameObject NewPartition(Vector2 position, Realm opponentRealm, int nbPieces, GameplayControler gameplayControler, int airLines)
    {
        var tmpOldPartition = GameObject.Find(Constants.GoPartition);
        if (tmpOldPartition != null)
            Destroy(tmpOldPartition);

        var tmpPartitionObject = Resources.Load<GameObject>("Prefabs/Partition");
        var tmpPartitionInstance = Instantiate(tmpPartitionObject, position, tmpPartitionObject.transform.rotation);
        tmpPartitionInstance.GetComponent<MusicPartitionBhv>().Init(opponentRealm, nbPieces, this, gameplayControler, airLines);
        tmpPartitionInstance.name = Constants.GoPartition;
        return tmpPartitionInstance;
    }

    public GameObject NewPartitionNote(Vector2 position)
    {
        var tmpPartitionNoteObject = Resources.Load<GameObject>("Prefabs/PartitionNote");
        var tmpPartitionNoteInstance = Instantiate(tmpPartitionNoteObject, position, tmpPartitionNoteObject.transform.rotation);
        return tmpPartitionNoteInstance;
    }

    public GameObject NewShiftBlock(Vector2 position, int nbRows, Realm opponentRealm, int nbColumns = 10)
    {
        var tmpShiftBlockObject = Resources.Load<GameObject>("Prefabs/ShiftBlock");
        var tmpShiftBlockInstance = Instantiate(tmpShiftBlockObject, position, tmpShiftBlockObject.transform.rotation);
        tmpShiftBlockInstance.GetComponent<ShiftBlockBhv>().Init(nbRows, nbColumns, opponentRealm);
        return tmpShiftBlockInstance;
    }

    public void PopIcon(Sprite sprite, Vector2 position)
    {
        var tmpPoppingIconObject = Resources.Load<GameObject>("Prefabs/PoppingIcon");
        var tmpPoppingIconInstance = Instantiate(tmpPoppingIconObject, position, tmpPoppingIconObject.transform.rotation);
        tmpPoppingIconInstance.GetComponent<PoppingIconBhv>().Init(sprite, position + new Vector2(0.0f, +0.3f));
    }

    public void NewDialogBoxEncounter(Vector3 position, string subjectName, string secondaryName, Realm secondaryRealm, Action resultAction, int? customId = null, string customDialogLibelle = null)
    {
        var tmpDialogBoxObject = Resources.Load<GameObject>("Prefabs/DialogBox");
        var tmpDialogBoxInstance = Instantiate(tmpDialogBoxObject, tmpDialogBoxObject.transform.position, tmpDialogBoxObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpDialogBoxInstance.name);
        tmpDialogBoxInstance.GetComponent<DialogBoxBhv>().Init(position, subjectName, secondaryName, secondaryRealm, resultAction, customId, customDialogLibelle);
    }

    public void NewDialogBoxDeath(Vector3 position, string name, Action resultAction)
    {
        var tmpDialogBoxObject = Resources.Load<GameObject>("Prefabs/DialogBox");
        var tmpDialogBoxInstance = Instantiate(tmpDialogBoxObject, tmpDialogBoxObject.transform.position, tmpDialogBoxObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpDialogBoxInstance.name);
        tmpDialogBoxInstance.GetComponent<DialogBoxBhv>().Init(position, name, null, Realm.None, resultAction);
    }

    public void NewFightIntro(Vector3 position, Character character, List<Opponent> opponents, Action resultAction)
    {
        var tmpFightIntroObject = Resources.Load<GameObject>("Prefabs/FightIntro");
        var tmpFightIntroInstance = Instantiate(tmpFightIntroObject, position, tmpFightIntroObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpFightIntroInstance.name);
        tmpFightIntroInstance.GetComponent<FightIntroBhv>().Init(character, opponents, resultAction);
    }

    public GameObject NewSimpShield(Vector3 parentPosition, int id, Realm realm)
    {
        var line = id / 3;
        var column = id % 3;
        var localPosition = new Vector3(-1.5f + (1.5f * column), -2.6f + (1.5f * line), 0.0f);
        var tmpShieldObject = Resources.Load<GameObject>("Prefabs/SimpShield");
        var tmpShieldInstance = Instantiate(tmpShieldObject, parentPosition + localPosition, tmpShieldObject.transform.rotation);
        tmpShieldInstance.name = Constants.GoSimpShield;
        tmpShieldInstance.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/Drone_{6 + (int)realm}");
        tmpShieldInstance.GetComponent<SpriteRenderer>().sortingOrder = id;
        return tmpShieldInstance;
    }

    public FillTargetBhv NewFillTarget(Realm realm, int nbBlocks, GameplayControler gameplayControler)
    {
        var tmpTargetObject = Resources.Load<GameObject>("Prefabs/DrillTarget");
        var tmpTargetInstance = Instantiate(tmpTargetObject, new Vector3(9.5f, -0.5f), tmpTargetObject.transform.rotation);
        tmpTargetInstance.name = Constants.GoFilledTarget;
        tmpTargetInstance.GetComponent<SpriteRenderer>().color = (Color)Constants.GetColorFromRealm(realm, 3);
        var fillTargetBhv = tmpTargetInstance.GetComponent<FillTargetBhv>();
        fillTargetBhv.Init(realm, nbBlocks, gameplayControler);
        return fillTargetBhv;
    }

    public BasketballHoopBhv NewHoop(GameplayControler gameplayControler)
    {
        var tmpHoopObject = Resources.Load<GameObject>("Prefabs/BasketballHoop");
        var tmpHoopInstance = Instantiate(tmpHoopObject, tmpHoopObject.transform.position, tmpHoopObject.transform.rotation);
        tmpHoopInstance.name = Constants.GoBasketballHoop;
        var hoopBhv = tmpHoopInstance.GetComponent<BasketballHoopBhv>();
        hoopBhv.Init(gameplayControler);
        return hoopBhv;
    }

    public GameObject NewHeightLimiter(int height, Realm realm, Transform parent)
    {
        var tmpLimiterObject = Resources.Load<GameObject>("Prefabs/HeightLimiter");
        var tmpLimiterInstance = Instantiate(tmpLimiterObject, new Vector3(4.5f, 0.0f, 0.0f), tmpLimiterObject.transform.rotation);
        tmpLimiterInstance.transform.SetParent(parent);
        tmpLimiterInstance.GetComponent<HeightLimiterBhv>().Set(height, realm);
        return tmpLimiterInstance;
    }

    public void NewLockPieceEffects(Transform piece)
    {
        var reflectName = "ReflectBlock";
        Realm realm = Realm.None;
        if (piece.name.Contains(Realm.Hell.ToString()))
        {
            reflectName += Realm.Hell.ToString();
            realm = Realm.Hell;
        }
        else if (piece.name.Contains(Realm.Earth.ToString()))
        {
            reflectName += Realm.Earth.ToString();
            realm = Realm.Earth;
        }
        else if (piece.name.Contains(Realm.Heaven.ToString()))
        {
            reflectName += Realm.Heaven.ToString();
            realm = Realm.Earth;
        }
        else if (piece.name.Contains("Classic"))
        {
            reflectName += "Classic";
            realm = Realm.None;
        }

        var tmpParticlesObject = Resources.Load<GameObject>("Prefabs/LockPieceParticle");
        foreach (Transform child in piece)
        {
            int roundedX = Mathf.RoundToInt(child.position.x);
            int roundedY = Mathf.RoundToInt(child.position.y);
            NewReflectBlock(reflectName, new Vector3(roundedX, roundedY, 0.0f), speed: 0.1f, color: (Color)Constants.GetColorFromRealm(realm, realm == Realm.None ? 2 : 4));
        }
        for (int x = 0; x < 10; ++x)
        {
            var maxY = Cache.HeightLimiter - 1;
            foreach (Transform child in piece)
            {
                int roundedX = Mathf.RoundToInt(child.position.x);
                int roundedY = Mathf.RoundToInt(child.position.y);
                if (roundedX != x)
                    continue;
                if (roundedY > maxY)
                    maxY = roundedY;
            }
            if (maxY > Cache.HeightLimiter - 1)
            {
                var tmpParticlesInstance = Instantiate(tmpParticlesObject, new Vector3(x, maxY + 0.75f, 0.0f), tmpParticlesObject.transform.rotation);
                tmpParticlesInstance.GetComponent<LockPieceParticleBhv>().Init(realm);
            }
        }
    }

    public GameObject NewLoading()
    {
        if (_mainCamera == null)
            _mainCamera = Helper.GetMainCamera();
        Cache.InputLocked = true;
        var pos = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0.0f);
        var tmpLoadingObject = Resources.Load<GameObject>($"Prefabs/{Constants.GoRestLoading}");
        var tmpLoadingInstance = Instantiate(tmpLoadingObject, pos, tmpLoadingObject.transform.rotation);
        tmpLoadingInstance.name = Constants.GoRestLoading;
        return tmpLoadingInstance;
    }

    public GameObject NewLineBreakLimiter(Realm realm)
    {
        var tmpLimiterObject = Resources.Load<GameObject>($"Prefabs/{Constants.GoLineBreakLimiter}");
        var tmpLimiterInstance = Instantiate(tmpLimiterObject, new Vector3(4.5f, 0.0f, 0.0f), tmpLimiterObject.transform.rotation);
        tmpLimiterInstance.GetComponent<SpriteRenderer>().color = (Color)Constants.GetColorFromRealm(realm, 4);
        tmpLimiterInstance.name = Constants.GoLineBreakLimiter;
        return tmpLimiterInstance;
    }

    internal RhythmIndicatorBhv NewRhythmIndicator(Color color)
    {
        var tmpIndicatorObject = Resources.Load<GameObject>($"Prefabs/{Constants.GoRhythmIndicator}");
        var tmpIndicatorInstance = Instantiate(tmpIndicatorObject, tmpIndicatorObject.transform.position, tmpIndicatorObject.transform.rotation);
        tmpIndicatorInstance.name = Constants.GoRhythmIndicator;
        var rhythmIndicatorBhv = tmpIndicatorInstance.GetComponent<RhythmIndicatorBhv>();
        rhythmIndicatorBhv.ApplyColor(color);
        return rhythmIndicatorBhv;
    }

    public GameObject NewLurkerShop(Action<bool> resumeAction, Character character)
    {
        var tmpShopObject = Resources.Load<GameObject>("Prefabs/LurkerShop");
        var tmpShopInstance = Instantiate(tmpShopObject, tmpShopObject.transform.position, tmpShopObject.transform.rotation);
        Cache.IncreaseInputLayer(tmpShopInstance.name);
        tmpShopInstance.GetComponent<LurkerShopBhv>().Init(this, resumeAction, character);
        return tmpShopInstance;
    }

    public GameObject NewClickMe(Vector3 position, Transform parent)
    {
        var tmpClickMeObject = Resources.Load<GameObject>("Prefabs/ClickMe");
        var tmpClickMeInstance = Instantiate(tmpClickMeObject, position, tmpClickMeObject.transform.rotation);
        tmpClickMeInstance.name = Constants.GoClickMe;
        tmpClickMeInstance.transform.SetParent(parent.transform);
        tmpClickMeInstance.GetComponent<BounceBhv>().Init();
        return tmpClickMeInstance;
    }
}
