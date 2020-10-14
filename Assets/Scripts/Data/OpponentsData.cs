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
            HpMax = 222, Weakness = Weakness.None, DamagesOnWeakness = 0, Cooldown = 4,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 2),
                //new OpponentAttack(AttackType.DarkRow, 2),
                //new OpponentAttack(AttackType.WasteRow, 2, 1)
            },
            GravityLevel = 1, DifficultyWeight = 0,
            Lore = "Youngest of the dummies, he wishes one day to take the place of his older brother, and thereby manage the training of hell's forces."
        },
        new Opponent()
        {
            Id = 1, Kind = "Dark Dummy", Realm = Realm.Hell, Type = OpponentType.Elite,
            HpMax = 444, Weakness = Weakness.Combos, DamagesOnWeakness = 6, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 4, 6),
                new OpponentAttack(AttackType.ForcedPiece, 0, 1),
                new OpponentAttack(AttackType.EmptyRow, 2),
                new OpponentAttack(AttackType.ForcedPiece, -1, 1)},
            GravityLevel = 5, DifficultyWeight = 0,
            Lore = "Older children of the dummies, his will is to honor the family duty, and prevent his little brother to murder him in order to take his place."
        },
        new Opponent()
        {
            Id = 2, Kind = "Mother Dummy", Realm = Realm.Hell, Type = OpponentType.Champion,
            HpMax = 666, Weakness = Weakness.xLines, XLineWeakness = 4, DamagesOnWeakness = 30, Cooldown = 8,
            Immunity = Immunity.xLines, XLineImmunity = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 3),
                new OpponentAttack(AttackType.WasteRow, 3, 3),
                new OpponentAttack(AttackType.EmptyRow, 3),
                new OpponentAttack(AttackType.LightRow, 3, 20)},
            GravityLevel = 10, DifficultyWeight = 0,
            Lore = "Matriarch of the dummy family. Her reign over the hell's forces training is one of a kind. It's said she even trained the Antichrist himself!"
        },
        new Opponent()
        {
            Id = 3, Kind = "Lost Soul", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 40, Weakness = Weakness.None, DamagesOnWeakness = 0, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 1) },
            GravityLevel = 1, DifficultyWeight = 10
        },
        new Opponent()
        {
            Id = 4, Kind = "Warrior Soul", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.Combos, DamagesOnWeakness = 10, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 2) },
            GravityLevel = 2, DifficultyWeight = 20
        },
        new Opponent()
        {
            Id = 5, Kind = "Tormentor", Realm = Realm.Hell, Type = OpponentType.Elite,
            HpMax = 300, Weakness = Weakness.Combos, DamagesOnWeakness = 5, Cooldown = 20,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 2, 2) },
            GravityLevel = 3, DifficultyWeight = 40
        },
        new Opponent()
        {
            Id = 6, Kind = "Harpy", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 250, Weakness = Weakness.Twists, DamagesOnWeakness = 20, Cooldown = 2,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedPiece, -2, 0) },
            GravityLevel = 8, DifficultyWeight = 40
        },
        new Opponent()
        {
            Id = 7, Kind = "Iron Maid", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 120, Weakness = Weakness.Twists, DamagesOnWeakness = 20, Cooldown = 16,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 4) },
            GravityLevel = 5, DifficultyWeight = 30
        },
        new Opponent()
        {
            Id = 8, Kind = "Lesser Demon", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 80, Weakness = Weakness.None, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 1) },
            GravityLevel = 5, DifficultyWeight = 20
        }
    };

    public static List<Opponent> EarthOpponents = new List<Opponent>();

    public static List<Opponent> HeavenOpponents = new List<Opponent>();
}
