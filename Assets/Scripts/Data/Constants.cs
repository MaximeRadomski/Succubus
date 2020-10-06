﻿using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class Constants
{
    public const float Pixel = 0.1428f;
    public const float SceneWidth = 20.57f;
    public const float CameraSize = 18.2936f;
    public const float GravityDelay = 1.0f;
    public const int GravityDivider = 4;
    public const int LinesForLevel = 10;
    public const float LockDelay = 0.5f;
    public const float AfterDropDelay = 0.1f;
    public const int NumberOfAllowedMovesBeforeLock = 15;
    public const int PlayFieldHeight = 40;
    public const int PlayFieldWidth = 10;
    public static List<int> RoomDifficultyPerRealm = new List<int>() {80, 160, 320};
    public static List<int> RoomDifficultyPerRoom = new List<int>() { 10, 20, 30 };
    public static List<float> RoomDifficultyPerRarity = new List<float>() { 1.0f, 1.5f, 2.0f };
    public static Vector3 _cameraVerticalGameplayPosition = new Vector3(4.5f, 1.64f, -10.0f);
    public static Vector3 _cameraHorizontalGameplayPosition = new Vector3(4.5f, 9.64f, -10.0f);
    public const float MaximumVolumeMusic = 0.2f;
    public const float MaximumEffectsMusic = 1.0f;
    public const string PiecesLetters = "IJLOSTZ";
    public const string AvailableBodyPartsIds = "000102030405060708091011";

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
    public const string PpUnlockedCharacters = "UnlockedCharacters";
    public const string PpUnlockedCharactersDefault = "110000000000";
    public const string PpTrainingScore = "TrainingScore";
    public const string PpTrainingLevel = "TrainingLevel";
    public const string PpTrainingLines = "TrainingLines";
    public const string PpTrainingPieces = "TrainingPieces";
    public const string PpTrainingHighScore = "TrainingHighScore";
    public const string PpGhostPieceColor = "GhostPieceColor";
    public const string PpGhostPieceColorDefault = "5";
    public const string PpGameplayChoice = "GameplayChoice";
    public const int PpGameplayChoiceDefault = 0;
    public const string PpButtonsLeftPanel = "PpButtonsLeftPanel";
    public const string PpButtonsLeftPanelDefault = "H000000D0000L0R0000d0";
    public const string PpButtonsRightPanel = "PpButtonsRightPanel";
    public const string PpButtonsRightPanelDefault = "I0S0000D0000A0C000000";
    public const string PpRun = "Run";
    public const string PpRunCharacter = "RunCharacter";
    public const string PpBag = "Bag";
    public const string PpHolder = "Holder";
    public const string PpSerializeDefault = null;
    public const string PpFavKeyboardLayout = "FavKeyboard";
    public const string PpEffectsLevel = "EffectsLevel";
    public const string PpMusicLevel = "MusicLevel";
    public const float PpAudioLevelDefault = 1.0f;
    public const int PpFavKeyboardLayoutDefault = 0;
    public const string PpTouchSensitivity = "TouchSensitivity";
    public const float PpTouchSensitivityDefault = 2.0f;

    //  SCENES  //
    public const string SplashScreenScene = "SplashScreenScene";
    public const string MainMenuScene = "MainMenuScene";
    public const string SettingsScene= "SettingsScene";
    public const string SettingsAudioScene = "SettingsAudioScene";
    public const string SettingsGameplayScene = "SettingsGameplayScene";
    public const string StepsScene = "StepsScene";
    public const string GameRogueScene = "GameRogueScene";
    public const string TrainingChoiceScene = "TrainingChoiceScene";
    public const string TrainingFreeGameScene = "TrainingFreeGameScene";
    public const string ClassicGameScene = "ClassicGameScene";
    public const string CharSelScene = "CharSelScene";

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
    public const string GoButtonPauseName = "ButtonPause";
    public const string GoButtonPlayName = "ButtonPlay";
    public const string GoButtonBackName = "ButtonBack";
    public const string GoButtonInfoName = "ButtonInfo";
    public const string GoMusicControler = "MusicControler";
    public const string GoCharacterInstance = "CharacterInstance";
    public const string GoOpponentInstance = "OpponentInstance";
    public const string GoForcedPiece = "ForcedPiece";


    //  GAMEOBJECT VALUES  //
    public const float KeyboardHeight = 3.1f;

    //  COLORS  //
    public static Color ColorHell0 = new Color(0.188f, 0.039f, 0.168f, 1.0f);  //300a2b
    public static Color ColorHell1 = new Color(0.392f, 0.035f, 0.141f, 1.0f);  //640924
    public static Color ColorHell2 = new Color(0.568f, 0.027f, 0.156f, 1.0f);  //910728
    public static Color ColorHell3 = new Color(0.752f, 0.094f, 0.156f, 1.0f);  //c01828
    public static Color ColorHell4 = new Color(0.901f, 0.254f, 0.215f, 1.0f);  //e64137

    public static Color ColorEarth0 = new Color(0.188f, 0.039f, 0.168f, 1.0f);  //300a2b
    public static Color ColorEarth1 = new Color(0.392f, 0.035f, 0.141f, 1.0f);  //640924
    public static Color ColorEarth2 = new Color(0.568f, 0.027f, 0.156f, 1.0f);  //910728
    public static Color ColorEarth3 = new Color(0.752f, 0.094f, 0.156f, 1.0f);  //c01828
    public static Color ColorEarth4 = new Color(0.901f, 0.254f, 0.215f, 1.0f);  //e64137

    public static Color ColorHeaven0 = new Color(0.188f, 0.039f, 0.168f, 1.0f);  //300a2b
    public static Color ColorHeaven1 = new Color(0.392f, 0.035f, 0.141f, 1.0f);  //640924
    public static Color ColorHeaven2 = new Color(0.568f, 0.027f, 0.156f, 1.0f);  //910728
    public static Color ColorHeaven3 = new Color(0.752f, 0.094f, 0.156f, 1.0f);  //c01828
    public static Color ColorHeaven4 = new Color(0.901f, 0.254f, 0.215f, 1.0f);  //e64137

    public static Color ColorTransparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    public static Color ColorPlain = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public static Color ColorPlainTransparent = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    public static Color ColorPlainSemiTransparent = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    public static Color ColorPlainQuarterTransparent = new Color(1.0f, 1.0f, 1.0f, 0.25f);

    public static string MaterialHell_4_3 = "<material=\"hell.4.3\">";
    public static string MaterialHell_3_2 = "<material=\"hell.3.2\">";
    public static string MaterialEnd = "</material>";

    public static object GetColorFromNature(Realm realm, int id)
    {
        if (id == -1)
            return Color.black;
        if (id == 5)
            return Color.white;
        var myActualType = typeof(Constants);
        var tmp = myActualType.GetField("Color" + realm.ToString() + id);
        return tmp.GetValue(myActualType);
    }

    // CACHE SAVES
    public static bool InputLocked = false;
    public static bool DoubleClick = false;
    public static int InputLayer = 0;
    public static string LastEndActionClickedName = null;
    public static string ClickHistory = null;
    public static List<string> InputTopLayerNames = null;
    public static GameMode CurrentGameMode;
    public static string NameLastScene;
    public static MusicType CurrentMusicType = MusicType.SplashScreen;

    //CACHE CLASSIC GAME
    public static int SelectedCharacterSpecialCooldown;
    public static int CurrentItemCooldown;
    public static int CurrentListOpponentsId;
    public static int CurrentOpponentHp;
    public static int CurrentOpponentCooldown;

    public static void ResetClassicGameCache()
    {
        ResetSelectedCharacterSpecialCooldown();
        CurrentListOpponentsId = 0;
        CurrentOpponentHp = 0;
        CurrentOpponentCooldown = 0;
    }

    public static void ResetSelectedCharacterSpecialCooldown()
    {
        var tmpChar = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()];
        Constants.SelectedCharacterSpecialCooldown = tmpChar.Cooldown - tmpChar.SpecialMaxCooldownReducer;
    }

    public static void ResetCurrentItemCooldown()
    {
        var tmpChar = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()];
        var tmpCurrentItem = PlayerPrefsHelper.GetCurrentItem();
        if (tmpCurrentItem == null)
            return;
        CurrentItemCooldown = tmpCurrentItem.Cooldown - tmpChar.ItemMaxCooldownReducer;
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
