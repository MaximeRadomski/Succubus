using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OpponentsData
{
    //DEBUG
    public static bool DebugEnabled = false;
    public static Opponent DebugOpponent = HellOpponents?.Find(o => o.Kind.Contains("Crossed"));

    static OpponentsData()
    {
        List<Opponent> realmOpponents = null;
        for (int realmId = 0; realmId < Helper.EnumCount<Realm>(); ++realmId)
        {
            if (realmId == Realm.Hell.GetHashCode())
                realmOpponents = HellOpponents;
            else if (realmId == Realm.Earth.GetHashCode())
                realmOpponents = EarthOpponents;
            else if (realmId == Realm.Heaven.GetHashCode())
                realmOpponents = HeavenOpponents;
            for (int i = 0; i < realmOpponents.Count; i++)
                realmOpponents[i].Id = i;
        }
    }

    public static List<Opponent> HellOpponents = new List<Opponent>()
    {
        new Opponent()
        {
            Kind = "Dummy", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 222, Weakness = Weakness.None, DamagesOnWeakness = 0, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 2),
                new OpponentAttack(AttackType.WasteRow, 2, 1)
            },
            GravityLevel = 2, DifficultyWeight = 0,
            Lore = "Youngest of the dummies, he wishes one day to take the place of his older brother, and thereby manage the training of hell's forces."
        },
        new Opponent()
        {
            Kind = "Dark Dummy", Realm = Realm.Hell, Type = OpponentType.Elite,
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
            Kind = "Mother Dummy", Realm = Realm.Hell, Type = OpponentType.Champion,
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
            Kind = "Lost Soul", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 40, Weakness = Weakness.None, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 1) },
            GravityLevel = 1, DifficultyWeight = 10
        },
        new Opponent()
        {
            Kind = "Ghoul", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 30, Weakness = Weakness.Consecutive, DamagesOnWeakness = 20, Cooldown = 3, Immunity = Immunity.xLines, XLineImmunity = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 1) },
            GravityLevel = 2, DifficultyWeight = 10
        },
        new Opponent()
        {
            Kind = "Hell Hound", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 20, Weakness = Weakness.None, Cooldown = 6,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.AirPiece, 1) },
            GravityLevel = 3, DifficultyWeight = 20
        },
        new Opponent()
        {
            Kind = "Warrior Soul", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.Combos, DamagesOnWeakness = 10, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 3) },
            GravityLevel = 5, DifficultyWeight = 20
        },
        new Opponent()
        {
            Kind = "Reverse Centaur", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 60, Weakness = Weakness.Twists, DamagesOnWeakness = 70, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.MirrorMirror, 3, 1) },
            GravityLevel = 5, DifficultyWeight = 30
        },
        new Opponent()
        {
            Kind = "Fire Cultist", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 80, Weakness = Weakness.Combos, DamagesOnWeakness = 5, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 3, 3) },
            GravityLevel = 1, DifficultyWeight = 30
        },
        new Opponent()
        {
            Kind = "Ghoul Rider", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 1, 4),
                new OpponentAttack(AttackType.EmptyRow, 4),
            },
            GravityLevel = 6, DifficultyWeight = 40
        },
        new Opponent()
        {
            Kind = "Harpy", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 170, Weakness = Weakness.Twists, DamagesOnWeakness = 20, Cooldown = 2,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedPiece, -2, 0) },
            GravityLevel = 8, DifficultyWeight = 40
        },
        new Opponent()
        {
            Kind = "Tormentor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.Combos, DamagesOnWeakness = 5, Cooldown = 20,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 2, 2) },
            GravityLevel = 5, DifficultyWeight = 40
        },
        new Opponent()
        {
            Kind = "Iron Maid", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 180, Weakness = Weakness.Twists, DamagesOnWeakness = 20, Cooldown = 16,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 4) },
            GravityLevel = 5, DifficultyWeight = 40
        },
        new Opponent()
        {
            Kind = "Snek Jailor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 2) },
            GravityLevel = 8, DifficultyWeight = 40
        },
        new Opponent()
        {
            Kind = "Crossed Demon", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.None, Cooldown = 16,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 3, 1) },
            GravityLevel = 1, DifficultyWeight = 50
        },
        new Opponent()
        {
            Kind = "Bad Moose", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.xLines, XLineWeakness = 2, DamagesOnWeakness = 20, Cooldown = 7,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LightRow, 2, 10) },
            GravityLevel = 1, DifficultyWeight = 40
        },
        new Opponent()
        {
            Kind = "Caca Demon", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.Consecutive, DamagesOnWeakness = 20, Cooldown = 15,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedPiece, -1, -1) },
            GravityLevel = 10, DifficultyWeight = 60
        },
        new Opponent()
        {
            Kind = "Shop Keeper", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 15,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.AirPiece, 3) },
            GravityLevel = 10, DifficultyWeight = 60
        },
        new Opponent()
        {
            Kind = "Baby Cthulhu", Realm = Realm.Earth, Type = OpponentType.Elite,
            HpMax = 999, Weakness = Weakness.None, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shift, 12) },
            GravityLevel = 15, DifficultyWeight = 100
        },
        new Opponent()
        {
            Kind = "Impostor", Realm = Realm.Hell, Type = OpponentType.Elite,
            HpMax = 50, Weakness = Weakness.None, Immunity = Immunity.xLines, XLineImmunity = 1, Cooldown = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 0) },
            GravityLevel = 15, DifficultyWeight = 100
        },
        new Opponent()
        {
            Kind = "Truth", Realm = Realm.Heaven, Type = OpponentType.Champion,
            HpMax = 1, Weakness = Weakness.None, Cooldown = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 1, 4) },
            GravityLevel = 20, DifficultyWeight = 100
        },
        new Opponent()
        {
            Kind = "Baphomeh", Realm = Realm.Hell, Type = OpponentType.Boss,
            HpMax = 500, Weakness = Weakness.None, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drone, 3, 1) },
            GravityLevel = 6, DifficultyWeight = 150
        },
        new Opponent()
        {
            Kind = "Devil's Advocate", Realm = Realm.Hell, Type = OpponentType.Boss,
            HpMax = 500, Weakness = Weakness.None, Cooldown = 4,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shift, 4) },
            GravityLevel = 10, DifficultyWeight = 150
        },
        new Opponent()
        {
            Kind = "Stan", Realm = Realm.Hell, Type = OpponentType.Boss,
            HpMax = 666, Weakness = Weakness.None, Immunity = Immunity.xLines, XLineImmunity = 4, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Gate, 3) },
            GravityLevel = 10, DifficultyWeight = 150
        }
    };

    public static List<Opponent> EarthOpponents = new List<Opponent>();

    public static List<Opponent> HeavenOpponents = new List<Opponent>();
}
