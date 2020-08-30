using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent
{
    public int Id;
    public string Kind;
    public int HpMax;
    public int Cooldown;
    public Realm Realm;
    public Weakness Weakness;
    public Immunity Immunity;
    public OpponentType Type;
    public int XLineWeakness;
    public int XLineImmunity;
    public int GravityLevel;

    public List<OpponentAttack> Attacks;

    public int DamagesOnWeakness;
    public bool IsDead;
    public int DifficultyWeight;

    public string Lore;
}
