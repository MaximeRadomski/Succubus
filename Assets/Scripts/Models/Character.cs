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

    public string TextEntrance;
    public string TextSpecial;
    public string TextVictory;
    public string TextDeath;

    //Global to all
    public int CritChancePercent;
    public int CritMultiplier;
    public int DamagePercentToInferiorRealm;
    public int DamagePercentToSuperiorRealm;
    public int RealmPassiveEffect;
    public int EnemyCooldownProgressionReducer;

    public Character()
    {
        CritChancePercent = 1;
        CritMultiplier = 50;
        DamagePercentToInferiorRealm = 20;
        DamagePercentToSuperiorRealm = 20;
        RealmPassiveEffect = 1;
        EnemyCooldownProgressionReducer = 0;
    }
}
