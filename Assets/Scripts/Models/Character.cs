using UnityEngine;

public class Character : Loot
{
    //Unique per character
    public int Id;
    public string Name;
    public string Kind;
    public int Attack;
    public int BoostAttack;
    public int Cooldown;
    public string SpecialName;
    public string SpecialDescription;

    public Realm Realm;
    public Realm StartingRealm;

    public string Lore;
    public int DialogId = 0;
    public float DialogPitch = 1.0f;

    //Global to all
    public int CritChancePercent = 1;
    public int CritMultiplier = 50;
    public int SingleLineDamageBonus = 0;
    public int DamagePercentBonus = 0;
    public int DamageFlatBonus = 0;
    public int DamagePercentToInferiorRealm = 20;
    public int RealmPassiveEffect = 1;
    public int EnemyCooldownProgressionReducer = 0;
    public float EnemyMaxCooldownMalus = 0;
    public int VisionBlockReducer = 1;
    public float ItemCooldownReducer = 1.0f;
    public int ItemMaxCooldownReducer = 0;
    public int ItemCooldownReducerOnKill = 0;
    public int SpecialCooldownReducer = 1;
    public int SpecialTotalCooldownReducer = 0;
    public int LandLordLateAmount = 1; //1 beause he does not land vision on first step
    public int AirPieceOpacity = 0;
    public bool CanMimic = false;
    public bool CanDoubleJump = false;
    public int PiecesWeight = 0;
    public int CumulativeCrit = 0;
    public int LoweredGravity = 0;
    public int TWorshipPercent = 0;
    public int IWorshipPercent = 0;
    public bool XRay = false;
    public int ComboDarkRow = 0;
    public int TrashAfterKill = 0;
    public bool HighPlayPause = false;
    public int ThornsPercent = 0;
    public int PerfectKills = 0;
    public int MaxDarkAndWasteLines = 99;
    public int BonusLife = 0;
    public int SimpShield = 0;
    public int DodgeChance = 0;
    public int FireDamagePercent = 0;
    public int EarthStun = 0;
    public int WaterDamagePercent = 0;
    public int WindTripleBonus = 0;
    public int QuadDamage = 0;
    public int DoubleEdgeGravity = 0;
    public int DamoclesDamage = 0;
    public int StepsWeightMalus = 0;
    public bool InstantSpecial = false;
    public bool LineDestroyInvulnerability = false;
    public int EnemyCooldownInfiniteStairMalus = 0;
    public bool HasteCancel = false;
    public bool HasteForAll = false;
    public int WasteHoleFiller = 0;
    public int DamageSmallLinesBonus = 0;
    public int DamageBigLinesBonus = 0;
    public int DamageSmallLinesMalus = 0;
    public int DamageBigLinesMalus = 0;
    public int GodHandCombo = 0;
    public bool MapAquired = false;
    public int FillTargetBlocks = 0;
    public int LastStandMultiplier = 0;
    public bool SlumberingDragoncrest = false;
    public bool SocialPyramid = false;
    public int ResourceFarmBonus = 0;
    public int OwlReduceSeconds = 0;
    public int ChanceAdditionalBlock = 0;
    public int DiamondBlocks = 0;
    public int CancelableShrinkingLines = 0;
    public int GateWidener = 0;
    public int BasketballHoopTimesBonus = 0;
    public int SlavWheelDamagePercentBonus = 0;
    public int DevilsContractMalus = 0;
    public int BassGuitarBonus = 0;
    public int NoodleShield = 0;
    public bool AllClear = false;
    public int InstantLineClear = 0;
    public int HolyMantle = 0;
    public int HeldBoosted = 0;
    public int Ying = 0;
    public int Yang = 0;
    public int SingleLinesDamageOverride = 0;
    public int QuadrupleLinesDamageOverride = 0;

    [System.NonSerialized]
    private int? _realmTreeAttackBoost;

    public Character()
    {
        LootType = LootType.Character;
    }

    public int GetAttack()
    {
        if (BoostAttack < 0)
            BoostAttack = 0;
        return GetAttackNoBoost() + BoostAttack;
    }

    public int GetAttackNoBoost()
    {
        if (!_realmTreeAttackBoost.HasValue)
            _realmTreeAttackBoost = PlayerPrefsHelper.GetRealmTree().AttackBoost;
        var flatDamage = Attack + DamageFlatBonus + Cache.DamoclesDamage + Cache.PactFlatDamage + _realmTreeAttackBoost.Value;
        if (Ying > 0 && Cache.SelectedCharacterSpecialCooldown <= 0)
            flatDamage += Ying;
        if (Yang > 0 && Cache.CurrentItemCooldown <= 0)
            flatDamage += Yang;
        var multiplierDamage = DamagePercentBonus;
        var floatValue = flatDamage * Helper.MultiplierFromPercent(1.0f, multiplierDamage);

        return Mathf.RoundToInt(floatValue);
    }
}
