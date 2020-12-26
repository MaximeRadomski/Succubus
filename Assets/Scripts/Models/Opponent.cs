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
    public bool TrainingOpponent;

    public List<OpponentAttack> Attacks;

    public int DamagesOnWeakness;
    public bool IsDead;
    public int DifficultyWeight;

    public string Lore;

    public Opponent Clone()
    {
        var newOpponent = new Opponent();

        newOpponent.Id = Id;
        newOpponent.Kind = Kind;
        newOpponent.HpMax = HpMax;
        newOpponent.Cooldown = Cooldown;
        newOpponent.Realm = Realm;
        newOpponent.Weakness = Weakness;
        newOpponent.Immunity = Immunity;
        newOpponent.Type = Type;
        newOpponent.XLineWeakness = XLineWeakness;
        newOpponent.XLineImmunity = XLineImmunity;
        newOpponent.GravityLevel = GravityLevel;
        newOpponent.Attacks = Attacks;
        newOpponent.DamagesOnWeakness = DamagesOnWeakness;
        newOpponent.IsDead = IsDead;
        newOpponent.DifficultyWeight = DifficultyWeight;
        newOpponent.Lore = Lore;

        return newOpponent;
    }
}
