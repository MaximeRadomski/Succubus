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

    //Global to all
    public int DamagePercentToInferiorRealm;
    public int DamagePercentToSuperiorRealm;
    public int RealmPassiveEffect;
    public int EnemyCooldownProgressionReducer;

    public Character()
    {
        DamagePercentToInferiorRealm = 20;
        DamagePercentToSuperiorRealm = 20;
        RealmPassiveEffect = 1;
        EnemyCooldownProgressionReducer = 1;
    }
}
