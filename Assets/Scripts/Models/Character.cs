using UnityEngine;

public class Character : Loot
{
    //Unique per character
    public int Id;
    public string Name;
    public string Kind;
    public int Attack;
    public int Cooldown;
    public string SpecialName;
    public string SpecialDescription;
    public Realm Realm;

    public string Lore;

    //Global to all
    public int CritChancePercent;
    public int CritMultiplier;
    public int SingleLineDamageBonus;
    public int DamagePercentBonus;
    public int DamagePercentToInferiorRealm;
    public int RealmPassiveEffect;
    public int EnemyCooldownProgressionReducer;
    public int VisionBlockReducer;
    public int ItemCooldownReducer;
    public int ItemMaxCooldownReducer;
    public int ItemCooldownReducerOnKill;
    public int SpecialCooldownReducer;
    public int SpecialMaxCooldownReducer;
    public int LandLordLateAmount;
    public bool CanMimic;

    public Character()
    {
        LootType = LootType.Character;
        CritChancePercent = 1;
        CritMultiplier = 50;
        SingleLineDamageBonus = 0;
        DamagePercentBonus = 0;
        DamagePercentToInferiorRealm = 20;
        RealmPassiveEffect = 1;
        EnemyCooldownProgressionReducer = 0;
        VisionBlockReducer = 1;
        ItemCooldownReducer = 1;
        ItemMaxCooldownReducer = 0;
        ItemCooldownReducerOnKill = 0;
        SpecialCooldownReducer = 1;
        SpecialMaxCooldownReducer = 0;
        LandLordLateAmount = 1; //1 beause he does not land vision on first step
        CanMimic = true;
    }

    public int GetAttack()
    {
        var floatValue = Attack * Helper.MultiplierFromPercent(1.0f, DamagePercentBonus);
        return Mathf.RoundToInt(floatValue);
    }
}
