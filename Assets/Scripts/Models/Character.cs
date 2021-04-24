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
    public int DeleteAfterKill = 0;
    public bool HighPlayPause = false;
    public int ThornsPercent = 0;
    public bool PerfectKills = false;
    public int MaxDarkAndWasteLines = 99;
    public int BonusLife = 0;
    public int SimpShield = 0;
    public int DodgeChance = 0;
    public int FireDamagesPercent = 0;
    public int EarthStun = 0;
    public int WaterDamagePercent = 0;
    public int WindTripleBonus = 0;
    public int QuadDamage = 0;
    public int DoubleEdgeGravity = 0;

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
        var floatValue = (Attack + DamageFlatBonus) * Helper.MultiplierFromPercent(1.0f, DamagePercentBonus);
        return Mathf.RoundToInt(floatValue);
    }
}
