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
            Id = 0, Kind = "Training Dummy", Realm = Realm.Hell,
            HP = 999, Weakness = Weakness.None, DamagesOnWeakness = 0, Cooldown = 25.0f,
            Attack1Type = AttackType.DarkLines, NbAttack1Rows = 4,
            Attack2Type = AttackType.GarbageLines, NbAttack2Rows = 4, Attack2Param = 1,
            Attack3Type = AttackType.LightLines, NbAttack3Rows = 4, Attack3Param = 10,
            GravityLevel = 4, DifficultyWeight = 10
        },
        new Opponent()
        {
            Id = 1, Kind = "Lost Soul", Realm = Realm.Hell,
            HP = 40, Weakness = Weakness.None, DamagesOnWeakness = 0, Cooldown = 10.0f,
            Attack1Type = AttackType.DarkLines, NbAttack1Rows = 1,
            GravityLevel = 1, DifficultyWeight = 10
        },
        new Opponent()
        {
            Id = 2, Kind = "Warrior Soul", Realm = Realm.Hell,
            HP = 100, Weakness = Weakness.Combos, DamagesOnWeakness = 10, Cooldown = 15.0f,
            Attack1Type = AttackType.DarkLines, NbAttack1Rows = 2,
            GravityLevel = 1, DifficultyWeight = 20
        },
        new Opponent()
        {
            Id = 3, Kind = "Tormentor", Realm = Realm.Hell,
            HP = 300, Weakness = Weakness.Combos, DamagesOnWeakness = 20, Cooldown = 30.0f,
            Attack1Type = AttackType.GarbageLines, NbAttack1Rows = 2,
            GravityLevel = 2, DifficultyWeight = 40
        }
    };
}
