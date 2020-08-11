using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent
{
    public int Id;
    public string Kind;
    public int Health;
    public AttackType AttackType;
    public int NbAttackRows;
    public float Cooldown;
    public Realm Realm;
    public Weakness Weakness;

    public int DamagesOnWeakness;
    public bool IsDead;
    public int DifficultyWeight;
}
