
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

    //CACHE CLASSIC GAME
    public static int SelectedCharacterSpecialCooldown;
    public static int CurrentItemCooldown;
    public static int CurrentItemUses;
    public static string GameOverParams = null;
    public static int CurrentOpponentAttackId;
    public static int CurrentRemainingSimpShields = 0;

    //Reset before each fight
    public static AttackType IsEffectAttackInProgress = AttackType.None;
    public static int CurrentListOpponentsId;
    public static int CurrentOpponentHp;
    public static float CurrentOpponentCooldown;
    public static int ComboCounter = 0;
    public static int CumulativeCrit = 0;
    public static int TripleLineDamageBonus = 0;
    public static int DamoclesDamage = 0;
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
    
    public static int PactFlatDamage = 0;
    public static bool PactCanHold = true;
    public static Realm PactCharacterRealm = Realm.None;
    public static int PactChanceAdditionalBlock = 0;

    public static void ResetClassicGameCache(Character character = null)
    {
        ResetSelectedCharacterSpecialCooldown(character);
        IsEffectAttackInProgress = AttackType.None;
        CurrentListOpponentsId = 0;
        CurrentOpponentHp = 0;
        CurrentOpponentCooldown = 0;
        CumulativeCrit = 0;
        TripleLineDamageBonus = 0;
        DamoclesDamage = 0;
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
        
        PactFlatDamage = 0;
        PactCanHold = true;
        PactCharacterRealm = Realm.None;
        PactChanceAdditionalBlock = 0;
    }

    public static void ResetSelectedCharacterSpecialCooldown(Character character)
    {
        Character tmpChar = character;
        if (tmpChar == null)
            tmpChar = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()];
        SelectedCharacterSpecialCooldown = tmpChar.Cooldown - tmpChar.SpecialTotalCooldownReducer;
        if (SelectedCharacterSpecialCooldown < 1)
            SelectedCharacterSpecialCooldown = 1;
        if (tmpChar.InstantSpecial)
            SelectedCharacterSpecialCooldown = 0;
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
