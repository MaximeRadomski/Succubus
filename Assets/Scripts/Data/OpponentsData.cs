using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OpponentsData
{
    public static List<Opponent> HellOpponents = new List<Opponent>()
    {
        new Opponent()
        {
            Id = 0, Kind = "Dummy", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 333, Weakness = Weakness.None, DamagesOnWeakness = 0, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRows, 1),
                new OpponentAttack(AttackType.GarbageRows, 1, 1),
                new OpponentAttack(AttackType.EmptyRows, 1),
                new OpponentAttack(AttackType.LightRows, 1, 10)},
            GravityLevel = 1, DifficultyWeight = 0
        },
        new Opponent()
        {
            Id = 1, Kind = "Dark Dummy", Realm = Realm.Hell, Type = OpponentType.Elite,
            HpMax = 666, Weakness = Weakness.Combos, DamagesOnWeakness = 6, Cooldown = 7,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRows, 2),
                new OpponentAttack(AttackType.GarbageRows, 2, 2),
                new OpponentAttack(AttackType.EmptyRows, 2),
                new OpponentAttack(AttackType.LightRows, 2, 15)},
            GravityLevel = 3, DifficultyWeight = 0
        },
        new Opponent()
        {
            Id = 2, Kind = "Mother Dummy", Realm = Realm.Hell, Type = OpponentType.Champion,
            HpMax = 999, Weakness = Weakness.xLines, XLineWeakness = 4, DamagesOnWeakness = 30, Cooldown = 5,
            Immunity = Immunity.xLines, XLineImmunity = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRows, 3),
                new OpponentAttack(AttackType.GarbageRows, 3, 3),
                new OpponentAttack(AttackType.EmptyRows, 3),
                new OpponentAttack(AttackType.LightRows, 3, 20)},
            GravityLevel = 5, DifficultyWeight = 0
        },
        new Opponent()
        {
            Id = 3, Kind = "Lost Soul", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 40, Weakness = Weakness.None, DamagesOnWeakness = 0, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRows, 1) },
            GravityLevel = 1, DifficultyWeight = 10
        },
        new Opponent()
        {
            Id = 4, Kind = "Warrior Soul", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.Combos, DamagesOnWeakness = 10, Cooldown = 15,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRows, 2) },
            GravityLevel = 1, DifficultyWeight = 20
        },
        new Opponent()
        {
            Id = 5, Kind = "Tormentor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 300, Weakness = Weakness.Combos, DamagesOnWeakness = 20, Cooldown = 30,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.GarbageRows, 3, 2) },
            GravityLevel = 2, DifficultyWeight = 40
        }
    };

    public static List<Opponent> EarthOpponents = new List<Opponent>();

    public static List<Opponent> HeavenOpponents = new List<Opponent>();
}
