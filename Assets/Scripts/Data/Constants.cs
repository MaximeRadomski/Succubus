using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class Constants
{
    public const bool BetaMode = true;

    public const int MaxFps = 60;
    public const float Pixel = 0.14285f;
    public const float SceneWidth = 20.57f;
    public const float CameraSize = 18.2936f;
    public const float GravityDelay = 1.0f;
    public const int GravityDivider = 4;
    public const int LinesForLevel = 10;
    public const float LockDelay = 0.5f;
    public const float OpponentCooldownIncrement = 0.05f;
    public const float OpponentCooldownOneHour = 3600.0f;
    public const float AfterDropDelay = 0.05f;
    public const int NumberOfAllowedMovesBeforeLock = 15;
    public const int PlayFieldHeight = 40;
    public const int PlayFieldWidth = 10;
    public static List<int> RoomDifficultyPerRealm = new List<int>() { 80, 160, 320 };
    public static List<int> RoomDifficultyPerRoom = new List<int>() { 10, 20, 30 };
    public static List<float> RoomDifficultyPerRarity = new List<float>() { 1.0f, 1.5f, 2.0f };
    public static Vector3 _cameraVerticalGameplayPosition = new Vector3(4.5f, 1.64f, -10.0f);
    public static Vector3 _cameraHorizontalGameplayPosition = new Vector3(4.5f, 9.64f, -10.0f);
    public const float MaximumVolumeMusic = 0.2f;
    public const float MaximumEffectsMusic = 1.0f;
    public const string PiecesLetters = "IJLOSTZ";
    public const string AvailableBodyPartsIds = "000102030405060708091011";
    public const float BaseButtonVisionConeMult = 1.5f;
    public const float OpponentUpgradeReducedCooldown = 0.20f;
    public const float OpponentUpgradeAddedLife = 0.15f;
    public const string DialogAudioClip = "Devlog";
    public const string IntroAudioClip = "Intro";
    public const string VictoryAudioClip = "Victory";
    public const int MaxRarePercent = 25;
    public const int MaxLegendaryPercent = 15;
    

    //  TAGS  //
    public const string TagMusic = "Music";
    public const string TagPlayField = "PlayField";
    public const string TagBackground = "Background";
    public const string TagButton = "Button";
    public const string TagSoundControler = "SoundControler";
    public const string TagPoppingText = "PoppingText";
    public const string TagLightRows = "LightRows";
    public const string TagVisionBlock = "VisionBlock";

    // UNITS OF MEASURE //
    public const int HourInMinutes = 60;
    public const int DayInMinutes = 1440;

    // TEXTS //
    public const string YesNoTitle = "Caution!";
    public const string YesNoContent = "Are you sure you want to do this action? It's Repercussions are irreversible!";
    public const string Cancel = "Cancel";
    public const string Proceed = "Proceed";

    //  PLAYER PREFS  //
    public const string PpSelectedCharacter = "SelectedCharacter";
    public const string PpCurrentOpponents = "CurrentOpponents";
    public const int PpSelectedCharacterDefault = 0;
    public const string PpCurrentItem = "CurrentItem";
    public const string PpCurrentTattoos = "CurrentTattoos";
    public const string PpCurrentBodyParts = "CurrentBodyParts";
    public const string PpCurrentMaxedOutTattoos = "CurrentMaxedOutTattoos";
    public const string PpUnlockedCharacters = "UnlockedCharacters";
    public const string PpUnlockedCharactersDefault = "110000000000";
    public const string PpTrainingScore = "TrainingScore";
    public const string PpTrainingLevel = "TrainingLevel";
    public const string PpTrainingLines = "TrainingLines";
    public const string PpTrainingPieces = "TrainingPieces";
    public const string PpTrainingHighScoreHistory = "TrainingHighScoreHistory";
    public const string PpTrainingHighScoreContext = "TrainingHighScoreContext";
    public const string PpGhostPieceColor = "GhostPieceColor";
    public const string PpGhostPieceColorDefault = "5";
    public const string PpGameplayChoice = "GameplayChoice";
    public const int PpGameplayChoiceDefault = 0;
    public const string PpButtonsLeftPanel = "ButtonsLeftPanel";
    public const string PpButtonsLeftPanelDefault = "H000000D0000L0R0000d0";
    public const string PpButtonsRightPanel = "ButtonsRightPanel";
    public const string PpButtonsRightPanelDefault = "S0I0000D0000A0C000000";
    public const string PpKeyBinding = "KeyLayout";
    public static string PpKeyBindingDefault = GetDefaultKeyBinding();
    public const string PpRun = "Run";
    public const string PpRunCharacter = "RunCharacter";
    public const string PpBag = "Bag";
    public const string PpHolder = "Holder";
    public const string PpSerializeDefault = null;
    public const int PpSerializeDefaultInt = 0;
    public const string PpFavKeyboardLayout = "FavKeyboard";
    public const string PpEffectsLevel = "EffectsLevel";
    public const string PpMusicLevel = "MusicLevel";
    public const string PpVibrationEnabled = "VibrationEnabled";
    public const float PpAudioLevelDefault = 1.0f;
    public const bool PpVibrationEnabledDefault = true;
    public const int PpFavKeyboardLayoutDefault = 0;
    public const string PpTouchSensitivity = "TouchSensitivity";
    public const float PpTouchSensitivityDefault = 1.5f;
    public const string PpScanlinesHardness = "ScanlinesHardness";
    public const int PpScanlinesHardnessDefault = 5;
    public const string PpFullScreen = "FullScreen";
    public const bool PpFullScreenDefault = true;
    public const string PpResolution = "Resolution";
    public const int PpResolutionDefault = 8;
    public const string PpLastMaxResolution = "LastMaxResolution";
    public const int PpLastMaxResolutionDefault = 0;
    public const string PpIsInFight = "IsInFight";
    public const string PpDifficulty = "Difficulty";
    public const int PpDifficultyDefault = 1;
    public const string PpDas = "Das";
    public const int PpDasDefault = 10;
    public const string PpArr = "Arr";
    public const int PpArrDefault = 2;
    public const string PpRotationPoint = "RotationPoint";
    public const bool PpRotationPointDefault = false;
    public const string PpClassicPieces = "ClassicPieces";
    public const bool PpClassicPiecesDefault = false;
    public const string PpOrientation = "Orientation";
    public const Direction PpOrientationDefault = Direction.Vertical;
    public const string PpRunAlreadyDialog = "Orientation";
    public const string PpRunAlreadyDialogDefault = null;
    public const string PpBonusRarePercent = "BonusRarePercent";
    public const string PpBonusLegendaryPercent = "BonusLegendaryPercent";
    public const string PpRunBossVanquished = "RunBossVanquished";
    public const string PpRealmBossProgression = "RealmBossProgression";
    public const int PpRealmBossProgressionDefault = -1;
    public const string PpTotalResources = "TotalResources";
    public const string PpTotalResourcesDefault = null;
    public const string PpBoughtTreeNodes = "BoughtTreeNodes";
    public const string PpRealmTree = "RealmTree";
    public const string PpVersion = "Version";
    public const string PpVersionDefault = "Alpha.01.004";
    public const string PpInfernalUnlocked = "InfernalUnlocked";

    public static string GetDefaultKeyBinding()
    {
        return $"{KeyCode.UpArrow.GetHashCode()};{KeyCode.DownArrow.GetHashCode()};{KeyCode.LeftArrow.GetHashCode()};{KeyCode.RightArrow.GetHashCode()};{KeyCode.C.GetHashCode()};{KeyCode.X.GetHashCode()};{KeyCode.Space.GetHashCode()};{KeyCode.D.GetHashCode()};{KeyCode.S.GetHashCode()};{KeyCode.Escape.GetHashCode()};{KeyCode.UpArrow.GetHashCode()};{KeyCode.DownArrow.GetHashCode()};{KeyCode.LeftArrow.GetHashCode()};{KeyCode.RightArrow.GetHashCode()};{KeyCode.Return.GetHashCode()};{KeyCode.W.GetHashCode()};";
    }

    //  SCENES  //
    public const string SplashScreenScene = "SplashScreenScene";
    public const string MainMenuScene = "MainMenuScene";
    public const string SettingsScene= "SettingsScene";
    public const string SettingsAudioScene = "SettingsAudioScene";
    public const string SettingsGameplayScene = "SettingsGameplayScene";
    public const string SettingsInputsScene = "SettingsInputsScene";
    public const string SettingsDisplayScene = "SettingsDisplayScene";
    public const string SettingsHowToPlay = "SettingsHowToPlay";
    public const string StepsAscensionScene = "StepsAscensionScene";
    public const string StepsScene = "StepsScene";
    public const string GameRogueScene = "GameRogueScene";
    public const string TrainingChoiceScene = "TrainingChoiceScene";
    public const string TrainingFreeGameScene = "TrainingFreeGameScene";
    public const string ClassicGameScene = "ClassicGameScene";
    public const string CharSelScene = "CharSelScene";
    public const string TreeScene = "TreeScene";
    public const string DifficultyScene = "DifficultyScene";
    public const string DemoEndScene = "DemoEndScene";
    public const string HighScoreScene = "HighScoreScene";
    public const string GameOverScene = "GameOverScene";
    public const string LoreScene = "LoreScene";

    //  GAMEOBJECT NAMES  //
    public const string GoSceneBhvName = "SceneBhv";
    public const string GoSpawnerName = "Spawner";
    public const string GoHolderName = "Holder";
    public const string GoNextPieceName = "NextPiece";
    public const string GoButtonLeftName = "ButtonLeft";
    public const string GoButtonRightName = "ButtonRight";
    public const string GoButtonHoldName = "ButtonHold";
    public const string GoButtonDownName = "ButtonDown";
    public const string GoButtonDropName = "ButtonDrop";
    public const string GoButtonItemName = "ButtonItem";
    public const string GoButtonSpecialName = "ButtonSpecial";
    public const string GoButtonAntiClockName = "ButtonAntiClock";
    public const string GoButtonClockName = "ButtonClock";
    public const string GoButton180 = "Button180";
    public const string GoButtonPauseName = "ButtonPause";
    public const string GoButtonPlayName = "ButtonPlay";
    public const string GoButtonBackName = "ButtonBack";
    public const string GoButtonInfoName = "ButtonInfo";
    public const string GoInputControler = "EventSystem";
    public const string GoMusicControler = "MusicControler";
    public const string GoCharacterInstance = "CharacterInstance";
    public const string GoOpponentInstance = "OpponentInstance";
    public const string GoForcedPiece = "ForcedPiece";
    public const string GoDrillTarget = "DrillTarget";
    public const string GoDrone = "Drone";
    public const string GoMenuSelector = "MenuSelector";
    public const string GoRotationPoint = "RotationPoint";
    public const string GoSimpShield = "SimpShield";

    //  GAMEOBJECT VALUES  //
    public const float KeyboardHeight = 3.1f;

    //  COLORS  //
    public static Color ColorHell0 = new Color(0.188f, 0.039f, 0.168f, 1.0f);  //300a2b
    public static Color ColorHell1 = new Color(0.392f, 0.035f, 0.141f, 1.0f);  //640924
    public static Color ColorHell2 = new Color(0.568f, 0.027f, 0.156f, 1.0f);  //910728
    public static Color ColorHell3 = new Color(0.752f, 0.094f, 0.156f, 1.0f);  //c01828
    public static Color ColorHell4 = new Color(0.901f, 0.254f, 0.215f, 1.0f);  //e64137

    public static Color ColorEarth0 = new Color(0.039f, 0.149f, 0.223f, 1.0f);  //0a2639
    public static Color ColorEarth1 = new Color(0.035f, 0.266f, 0.274f, 1.0f);  //094446
    public static Color ColorEarth2 = new Color(0.019f, 0.415f, 0.341f, 1.0f);  //056a57
    public static Color ColorEarth3 = new Color(0.074f, 0.556f, 0.415f, 1.0f);  //138e6a
    public static Color ColorEarth4 = new Color(0.121f, 0.701f, 0.403f, 1.0f);  //1fb367

    public static Color ColorHeaven0 = new Color(0.141f, 0.101f, 0.286f, 1.0f);  //241a49
    public static Color ColorHeaven1 = new Color(0.215f, 0.121f, 0.545f, 1.0f);  //371f8b
    public static Color ColorHeaven2 = new Color(0.282f, 0.188f, 0.843f, 1.0f);  //4830d7
    public static Color ColorHeaven3 = new Color(0.039f, 0.450f, 0.819f, 1.0f);  //0a73d1
    public static Color ColorHeaven4 = new Color(0.023f, 0.647f, 0.898f, 1.0f);  //06a5e5

    public static Color ColorPlain = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public static Color ColorPlainTransparent = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    public static Color ColorPlainSemiTransparent = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    public static Color ColorPlainQuarterTransparent = new Color(1.0f, 1.0f, 1.0f, 0.25f);
    public static Color ColorBlack = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    public static Color ColorBlackTransparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    public static Color ColorBlackSemiTransparent = new Color(0.0f, 0.0f, 0.0f, 0.5f);

    public static string MaterialEnd = "</material>";

    public static object GetColorFromRealm(Realm realm, int id)
    {
        if (id == -1)
            return Color.black;
        if (id == 5)
            return Color.white;
        else if (realm == Realm.None)
        {
            return new Color(1.0f, 1.0f, 1.0f, (1.0f / 7) * (id + 1));
        }
        var myActualType = typeof(Constants);
        var tmp = myActualType.GetField("Color" + realm.ToString() + id);
        return tmp.GetValue(myActualType);
    }

    public static string GetMaterial(Realm realm, TextType type, TextCode code)
    {
        var realmStr = realm.ToString();
        var typeStr = "";
        if (type == TextType.AbjectLong)
            typeStr = "Long";
        else
            realmStr = realmStr.ToLower();
        var material = $"<material=\"{typeStr}{realmStr}{code.GetDescription()}\">";
        return material;
    }

    // CACHE SAVES
    public static bool InputLocked = false;
    public static bool EscapeLocked = false;
    public static bool DoubleClick = false;
    public static int InputLayer = 0;
    public static string LastEndActionClickedName = null;
    public static string ClickHistory = null;
    public static List<string> InputTopLayerNames = null;
    public static GameMode CurrentGameMode;
    public static string NameLastScene;
    public static List<int> CurrentHighScoreContext;
    public static bool OnlyMouseInMenu;

    //CACHE CLASSIC GAME
    public static int SelectedCharacterSpecialCooldown;
    public static int CurrentItemCooldown;
    public static int CurrentItemUses;
    public static bool IsffectAttackInProgress = false;
    public static string GameOverParams = null;
    public static int CurrentOpponentAttackId;
    public static int CurrentRemainingSimpShields = 0;

    //Reset before each fight
    public static int CurrentListOpponentsId;
    public static int CurrentOpponentHp;
    public static float CurrentOpponentCooldown;
    public static int CumulativeCrit = 0;
    public static int TripleLineDamageBonus = 0;
    public static int DamoclesDamages = 0;
    public static int ChanceAttacksHappeningPercent = 100;
    public static AttackType RandomizedAttackType = AttackType.None;
    public static bool HalvedCooldown = false;
    public static int AddedDodgeChancePercent = 0;
    public static int BlockPerAttack = -1;
    public static float BonusLockDelay = 0.0f;
    public static bool TruthResurection = false;
    public static Realm CurrentOpponentChangedRealm = Realm.None;
    public static int HeightLimiter = 0;
    public static int HeightLimiterResetLines = -1;

    public static void ResetClassicGameCache(Character character = null)
    {
        ResetSelectedCharacterSpecialCooldown(character);
        CurrentListOpponentsId = 0;
        CurrentOpponentHp = 0;
        CurrentOpponentCooldown = 0;
        CumulativeCrit = 0;
        TripleLineDamageBonus = 0;
        DamoclesDamages = 0;
        ChanceAttacksHappeningPercent = 100;
        RandomizedAttackType = AttackType.None;
        HalvedCooldown = false;
        AddedDodgeChancePercent = 0;
        BlockPerAttack = -1;
        BonusLockDelay = 0.0f;
        TruthResurection = false;
        CurrentOpponentChangedRealm = Realm.None;
        HeightLimiter = 0;
        HeightLimiterResetLines = -1;
    }

    public static void ResetSelectedCharacterSpecialCooldown(Character character)
    {
        Character tmpChar = character;
        if (tmpChar == null)
            tmpChar = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()];
        Constants.SelectedCharacterSpecialCooldown = tmpChar.Cooldown - tmpChar.SpecialTotalCooldownReducer;
        if (Constants.SelectedCharacterSpecialCooldown < 1)
            Constants.SelectedCharacterSpecialCooldown = 1;
    }

    public static void RestartCurrentItemCooldown(Character character, Item item = null)
    {
        Item tmpCurrentItem;
        if (item != null)
            tmpCurrentItem = item;
        else
            tmpCurrentItem = PlayerPrefsHelper.GetCurrentItem();
        if (tmpCurrentItem == null)
            return;
        CurrentItemCooldown = tmpCurrentItem.Cooldown - character.ItemMaxCooldownReducer;
    }

    public static void SetLastEndActionClickedName(string name)
    {
        if (!DoubleClick) //Prevents triple click
            ClickHistory = LastEndActionClickedName;
        LastEndActionClickedName = name;

        if (ClickHistory == LastEndActionClickedName)
        {
            DoubleClick = true;
            ClickHistory = string.Empty; //Prevents triple click
        }
        else
            DoubleClick = false;
    }

    public static void IncreaseInputLayer(string name)
    {
        ++InputLayer;
        if (InputTopLayerNames == null)
            InputTopLayerNames = new List<string>();
        InputTopLayerNames.Add(name);
    }

    public static void DecreaseInputLayer()
    {
        --InputLayer;
        if (InputTopLayerNames == null)
            return;
        if (InputTopLayerNames.Count <= 0)
        {
            InputLayer = 0;
            return;
        }
        InputTopLayerNames.RemoveAt(InputTopLayerNames.Count - 1);
    }
}
