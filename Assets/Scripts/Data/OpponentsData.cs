using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OpponentsData
{
    public static List<Opponent> Opponents = new List<Opponent>()
    {
        //HELL
        new Opponent()
        {
            Id = 0, Kind = "Training Dummy", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 999, Weakness = Weakness.None, DamagesOnWeakness = 0, Cooldown = 10.0f,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRows, 4),
                new OpponentAttack(AttackType.GarbageRows, 4, 1),
                new OpponentAttack(AttackType.LightRows, 4, 10),
                new OpponentAttack(AttackType.EmptyRows, 4)},
            GravityLevel = 4, DifficultyWeight = 0
        },
        new Opponent()
        {
            Id = 1, Kind = "Lost Soul", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 40, Weakness = Weakness.None, DamagesOnWeakness = 0, Cooldown = 10.0f,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRows, 1) },
            GravityLevel = 1, DifficultyWeight = 10
        },
        new Opponent()
        {
            Id = 2, Kind = "Warrior Soul", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.Combos, DamagesOnWeakness = 10, Cooldown = 15.0f,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRows, 2) },
            GravityLevel = 1, DifficultyWeight = 20
        },
        new Opponent()
        {
            Id = 3, Kind = "Tormentor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 300, Weakness = Weakness.Combos, DamagesOnWeakness = 20, Cooldown = 30.0f,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.GarbageRows, 3, 2) },
            GravityLevel = 2, DifficultyWeight = 40
        }
    };
}
