using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OpponentsData
{
    public static List<Opponent> Characters = new List<Opponent>()
    {
        //HELL
        new Opponent()
        {
            Id = 0, Kind = "Lost Soul", Realm = Realm.Hell,
            Health = 40, Weakness = Weakness.None, DamagesOnWeakness = 0,
            AttackType = AttackType.Black, NbAttackRows = 1, Cooldown = 10.0f,
            DifficultyWeight = 10
        },
        new Opponent()
        {
            Id = 1, Kind = "Warrior Soul", Realm = Realm.Hell,
            Health = 100, Weakness = Weakness.Combos, DamagesOnWeakness = 10,
            AttackType = AttackType.Black, NbAttackRows = 2, Cooldown = 15.0f,
            DifficultyWeight = 20
        },
        new Opponent()
        {
            Id = 2, Kind = "Tormentor", Realm = Realm.Hell,
            Health = 300, Weakness = Weakness.Combos, DamagesOnWeakness = 20,
            AttackType = AttackType.Grey, NbAttackRows = 2, Cooldown = 30.0f,
            DifficultyWeight = 40
        }
    };
}
