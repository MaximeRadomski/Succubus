using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent
{
    public int Id;
    public string Kind;
    public bool IsBoss;
    public int HP;
    public float Cooldown;
    public Realm Realm;
    public Weakness Weakness;
    public int GravityLevel;
    
    public AttackType Attack1Type;
    public int NbAttack1Rows;
    public int Attack1Param; //For Grey lines -> number of holes in each line (min 1). For Black lines -> Nothing. For White lines -> lines cooldown.

    public AttackType Attack2Type;
    public int NbAttack2Rows;
    public int Attack2Param;

    public AttackType Attack3Type;
    public int NbAttack3Rows;
    public int Attack3Param;

    public int DamagesOnWeakness;
    public bool IsDead;
    public int DifficultyWeight;

    public string Lore;
}
