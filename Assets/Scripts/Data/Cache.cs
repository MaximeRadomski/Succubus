
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class Cache
{
    // CACHE SAVES
    public static bool HasStartedBySplashScreen = false;
    public static int NetworkErrorCount = 0;
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
    public static bool KeyboardUp;
    public static int CurrentBossId;
    public static bool InAHurryPopup = false;

    //CACHE CLASSIC GAME
    public static int SelectedCharacterSpecialCooldown;
    public static int CurrentItemCooldown;
    public static int CurrentItemUses;
    public static string GameOverParams = null;
    public static int CurrentOpponentAttackId;
    public static int CurrentOpponentAttackCount;
    public static int CurrentRemainingSimpShields = 0;
    public static Vector3? HorizontalCameraInitialPosition;
    public static Quaternion? HorizontalCameraInitialRotation;
    public static int TrainingFreeSelectedLevel = 1;

    //Reset before each fight
    public static AttackType IsEffectAttackInProgress = AttackType.None;
    public static int CurrentListOpponentsId;
    public static int CurrentOpponentHp;
    public static float CurrentOpponentCooldown;
    public static int ComboCounter = 0;
    public static int CumulativeCrit = 0;
    public static int TripleLineDamageBonus = 0;
    public static int BonusDamage = 0;
    public static AttackType RandomizedAttackType = AttackType.None;
    public static bool HalvedCooldown = false;
    public static int AddedDodgeChancePercent = 0;
    public static int BlockPerAttack = -1;
    public static float BonusLockDelay = 0.0f;
    public static int TruthResurrection = 0;
    public static Realm CurrentOpponentChangedRealm = Realm.None;
    public static int HeightLimiter = 0;
    public static int HeightLimiterResetPieces = -1;
    public static Vector3? OnResumeLastPiecePosition;
    public static Quaternion? OnResumeLastPieceRotation;
    public static int? OnResumeLastForcedBlocks;
    public static bool IsNextOpponentAttackCanceled;
    public static int EnemyCooldownInfiniteStairMalus = 0;
    public static bool SlumberingDragoncrestInEffect = false;
    public static bool HasLastStanded = false;
    public static int CanceledShrinkingLines = 0;
    public static int CanceledDiamondBlocks = 0;
    public static int SlavWheelStreak = 0;
    public static int LineBreakReach = 0;
    public static int LineBreakCount = 0;
    public static int MusicAttackCount = 0;
    public static int NoodleShieldCount = 0;
    public static int HeldBoostedCount = 0;
    public static bool TwistBoostedPiece = false;
    public static int SheleredAttacksCount = 0;
    public static int NegateAttackBoostCount = 0;
    public static int CookieCount = 0;
    public static Character TemporaryCharacter = null;
    public static int GatlingPiecesCount = 0;
    public static int WhacAMoleAttackCount = 0;
    public static float ArcadeTime = 0.0f;

    public static int PactFlatDamage = 0;
    public static int PactCritChance = 0;
    public static int PactComboDamage = 0;
    public static int PactChanceAdditionalBlock = 0;
    public static int PactEnemyMaxCooldownMalus = 0;
    public static int PactSpecialTotalCooldownReducer = 0;
    public static bool PactNoLoot = false;
    public static bool PactNoCrit = false;
    public static bool PactNoHold = false;
    public static bool PactNoHaste = false;
    public static bool PactStealth = false;
    public static bool PactOnlyHaste = false;
    public static bool PactNoSoftDrop = false;
    public static bool PactZeroGravity = false;
    public static bool PactCooldownSwap = false;
    public static bool PactResurrection = false;
    public static bool PactNoLastFightPlayField = false;
    public static Realm PactCharacterRealm = Realm.None;

    public static int PlayFieldMinHeight => HeightLimiter + LineBreakCount;

    public static void ResetClassicGameCache(Character character = null)
    {
        IsEffectAttackInProgress = AttackType.None;
        CurrentListOpponentsId = 0;
        CurrentOpponentHp = 0;
        CurrentOpponentCooldown = 0;
        CumulativeCrit = 0;
        TripleLineDamageBonus = 0;
        BonusDamage = 0;
        RandomizedAttackType = AttackType.None;
        HalvedCooldown = false;
        AddedDodgeChancePercent = 0;
        BlockPerAttack = -1;
        BonusLockDelay = 0.0f;
        TruthResurrection = 0;
        CurrentOpponentChangedRealm = Realm.None;
        HeightLimiter = 0;
        HeightLimiterResetPieces = -1;
        OnResumeLastPiecePosition = null;
        OnResumeLastPieceRotation = null;
        OnResumeLastForcedBlocks = null;
        IsNextOpponentAttackCanceled = false;
        EnemyCooldownInfiniteStairMalus = 0;
        SlumberingDragoncrestInEffect = false;
        HasLastStanded = false;
        CanceledShrinkingLines = 0;
        CanceledDiamondBlocks = 0;
        SlavWheelStreak = 0;
        LineBreakReach = 0;
        LineBreakCount = 0;
        MusicAttackCount = 0;
        NoodleShieldCount = 0;
        HeldBoostedCount = 0;
        TwistBoostedPiece = false;
        SheleredAttacksCount = 0;
        NegateAttackBoostCount = 0;
        CookieCount = 0;
        TemporaryCharacter = null;
        GatlingPiecesCount = 0;
        WhacAMoleAttackCount = 0;
        ArcadeTime = 0.0f;

        PactFlatDamage = 0;
        PactCritChance = 0;
        PactComboDamage = 0;
        PactChanceAdditionalBlock = 0;
        PactEnemyMaxCooldownMalus = 0;
        PactSpecialTotalCooldownReducer = 0;
        PactNoHold = false;
        PactNoLoot = false;
        PactNoCrit = false;
        PactStealth = false;
        PactNoHaste = false;
        PactOnlyHaste = false;
        PactNoSoftDrop = false;
        PactZeroGravity = false;
        PactCooldownSwap = false;
        PactResurrection = false;
        PactNoLastFightPlayField = false;
        PactCharacterRealm = Realm.None;
        ResetSelectedCharacterSpecialCooldown(character);
    }

    public static void ResetSelectedCharacterSpecialCooldown(Character character, bool fromSpecial = false)
    {
        Character tmpChar = character;
        if (tmpChar == null)
            tmpChar = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()];
        SelectedCharacterSpecialCooldown = tmpChar.Cooldown - tmpChar.SpecialTotalCooldownReducer;
        if (PactSpecialTotalCooldownReducer > 0)
            SelectedCharacterSpecialCooldown -= PactSpecialTotalCooldownReducer;
        if (PactCooldownSwap)
            SelectedCharacterSpecialCooldown *= 2;
        if (SelectedCharacterSpecialCooldown < 1)
            SelectedCharacterSpecialCooldown = 1;
        if (!fromSpecial && tmpChar.InstantSpecial)
            SelectedCharacterSpecialCooldown = 0;
    }

    public static void RestartCurrentItemCooldown(Character character, Item item = null)
    {
        Item tmpCurrentItem;
        if (item != null)
            tmpCurrentItem = item;
        else
            tmpCurrentItem = PlayerPrefsHelper.GetCurrentItem();
        if (tmpCurrentItem == null || tmpCurrentItem.Type != ItemType.CooldownBased)
            return;
        CurrentItemCooldown = tmpCurrentItem.Cooldown - character.ItemMaxCooldownReducer;
        if (PactCooldownSwap)
            CurrentItemCooldown /= 2;
        if (CurrentItemCooldown < 1)
            CurrentItemCooldown = 1;
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
