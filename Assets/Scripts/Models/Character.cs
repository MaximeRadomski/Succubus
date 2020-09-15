using UnityEngine;

public class Character
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
    public int DamagePercentToInferiorRealm;
    public int RealmPassiveEffect;
    public int EnemyCooldownProgressionReducer;
    public int VisionBlockReducer;
    public int ItemCooldownReducer;
    public int ItemMaxCooldownReducer;
    public int SpecialCooldownReducer;
    public int SpecialMaxCooldownReducer;

    public Character()
    {
        CritChancePercent = 1;
        CritMultiplier = 50;
        DamagePercentToInferiorRealm = 20;
        RealmPassiveEffect = 1;
        EnemyCooldownProgressionReducer = 0;
        VisionBlockReducer = 1;
        ItemCooldownReducer = 1;
        ItemMaxCooldownReducer = 0;
        SpecialCooldownReducer = 1;
        SpecialMaxCooldownReducer = 0;
    }
}
