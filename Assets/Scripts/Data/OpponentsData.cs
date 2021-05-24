using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OpponentsData
{
    //DEBUG
    public static bool DebugEnabled = false;
    public static bool OnlyOpponent = true;
    public static Opponent DebugOpponent()
    {
        return HellOpponents?.Find(o => o.Name.Contains("Boom Slayer")).Clone();
    }

    static OpponentsData()
    {
        List<Opponent> realmOpponents = null;
        Realm? region = null;
        for (int realmId = 0; realmId < Helper.EnumCount<Realm>(); ++realmId)
        {
            if (realmId == Realm.Hell.GetHashCode())
            {
                realmOpponents = HellOpponents;
                region = Realm.Hell;
            }
            else if (realmId == Realm.Earth.GetHashCode())
            {
                realmOpponents = EarthOpponents;
                region = Realm.Earth;
            }
            else if (realmId == Realm.Heaven.GetHashCode())
            {
                realmOpponents = HeavenOpponents;
                region = Realm.Heaven;
            }
            for (int i = 0; i < realmOpponents.Count; i++)
            {
                realmOpponents[i].Id = i;
                realmOpponents[i].Region = region ?? Realm.Hell;
            }
        }
    }

    public static List<Opponent> HellOpponents = new List<Opponent>()
    {
        new Opponent()
        {
            Name = "Dummy", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 222, Weakness = Weakness.None, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 2),
                new OpponentAttack(AttackType.WasteRow, 2, 1)},
            GravityLevel = 2, Weight = 0,
            Lore = "Youngest of the dummies, he wishes one day to take the place of his older brother, and thereby manage the training of hell's forces."
        },
        new Opponent()
        {
            Name = "Dark Dummy", Realm = Realm.Hell, Type = OpponentType.Elite,
            HpMax = 444, Weakness = Weakness.Combos, DamagesOnWeakness = 6, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 4, 6),
                new OpponentAttack(AttackType.ForcedPiece, 0, 1),
                new OpponentAttack(AttackType.EmptyRow, 2),
                new OpponentAttack(AttackType.ForcedPiece, -1, 1)},
            GravityLevel = 5, Weight = 0,
            Lore = "Older children of the dummies, his will is to honor the family duty, and prevent his little brother to murder him in order to take his place."
        },
        new Opponent()
        {
            Name = "Mother Dummy", Realm = Realm.Hell, Type = OpponentType.Champion,
            HpMax = 666, Weakness = Weakness.xLines, XLineWeakness = 4, DamagesOnWeakness = 30, Cooldown = 8,
            Immunity = Immunity.xLines, XLineImmunity = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 3),
                new OpponentAttack(AttackType.WasteRow, 3, 3),
                new OpponentAttack(AttackType.EmptyRow, 3),
                new OpponentAttack(AttackType.LightRow, 3, 20)},
            GravityLevel = 10, Weight = 0,
            Lore = "Matriarch of the dummy family. Her reign over the hell's forces training is one of a kind. It's said she even trained the Antichrist himself!"
        },
        new Opponent()
        {
            Name = "Lost Soul", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 40, Weakness = Weakness.None, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 1) },
            GravityLevel = 1, Weight = 10
        },
        new Opponent()
        {
            Name = "Ghoul", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 30, Weakness = Weakness.Consecutive, DamagesOnWeakness = 20, Cooldown = 6, Immunity = Immunity.xLines, XLineImmunity = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 1) },
            GravityLevel = 2, Weight = 10
        },
        new Opponent()
        {
            Name = "Hell Hound", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 20, Weakness = Weakness.None, Cooldown = 6,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.AirPiece, 1) },
            GravityLevel = 2, Weight = 20
        },
        new Opponent()
        {
            Name = "Warrior Soul", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.Combos, DamagesOnWeakness = 10, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 2) },
            GravityLevel = 3, Weight = 20
        },
        new Opponent()
        {
            Name = "Reverse Centaur", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 60, Weakness = Weakness.Twists, DamagesOnWeakness = 70, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.MirrorMirror, 2, 1) },
            GravityLevel = 3, Weight = 30
        },
        new Opponent()
        {
            Name = "Fire Cultist", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 80, Weakness = Weakness.Combos, DamagesOnWeakness = 5, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 3, 5) },
            GravityLevel = 1, Weight = 30
        },
        new Opponent()
        {
            Name = "Headbanger", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 80, Weakness = Weakness.None, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Intoxication, 3) },
            GravityLevel = 3, Weight = 30
        },
        new Opponent()
        {
            Name = "Dark Coffee", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 60, Weakness = Weakness.Twists, DamagesOnWeakness = 50, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.MirrorMirror, 2, 0) },
            GravityLevel = 3, Weight = 30
        },
        new Opponent()
        {
            Name = "Bass Sinner", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 120, Weakness = Weakness.None, Cooldown = 2,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LightRow, 2, 8) },
            GravityLevel = 1, Weight = 40
        },
        new Opponent()
        {
            Name = "Giant Spider", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 99, Weakness = Weakness.Consecutive, DamagesOnWeakness = 20, Cooldown = 4,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drone, 1, 2) },
            GravityLevel = 5, Weight = 40
        },
        new Opponent()
        {
            Name = "Tax Collector", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.xLines, XLineWeakness = 2, DamagesOnWeakness = 50, Cooldown = 14, Immunity = Immunity.xLines, XLineImmunity = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 10, 8) },
            GravityLevel = 3, Weight = 40
        },
        new Opponent()
        {
            Name = "Hell Raider", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.Twists, DamagesOnWeakness = 40, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedPiece, -1, -1) },
            GravityLevel = 1, Weight = 40
        },
        new Opponent()
        {
            Name = "Cat", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 77, Cooldown = 3, Immunity = Immunity.Cooldown,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.EmptyRow, 2) },
            GravityLevel = 3, Weight = 40
        },
        new Opponent()
        {
            Name = "Ghoul Rider", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 6,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 1, 4),
                new OpponentAttack(AttackType.EmptyRow, 4),
            },
            GravityLevel = 4, Weight = 40
        },
        new Opponent()
        {
            Name = "Harpy", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 170, Weakness = Weakness.Twists, DamagesOnWeakness = 20, Cooldown = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedPiece, -2, 0) },
            GravityLevel = 5, Weight = 40
        },
        new Opponent()
        {
            Name = "Tormentor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.Combos, DamagesOnWeakness = 5, Cooldown = 15, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 2, 2) },
            GravityLevel = 3, Weight = 40
        },
        new Opponent()
        {
            Name = "Iron Maid", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 180, Weakness = Weakness.Twists, DamagesOnWeakness = 20, Cooldown = 15,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 4) },
            GravityLevel = 3, Weight = 40
        },
        new Opponent()
        {
            Name = "Snek Jailor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 3,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 2) },
            GravityLevel = 5, Weight = 40
        },
        new Opponent()
        {
            Name = "Crossed Demon", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.None, Cooldown = 16, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 3, 1) },
            GravityLevel = 1, Weight = 40
        },
        new Opponent()
        {
            Name = "Bad Moose", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.xLines, XLineWeakness = 2, DamagesOnWeakness = 20, Cooldown = 7,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LightRow, 2, 10) },
            GravityLevel = 1, Weight = 30
        },
        new Opponent()
        {
            Name = "Caca Demon", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 180, Weakness = Weakness.Consecutive, DamagesOnWeakness = 20, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedPiece, 3, 0) },
            GravityLevel = 6, Weight = 50
        },
        new Opponent()
        {
            Name = "Shop Keeper", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 15,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.AirPiece, 3) },
            GravityLevel = 6, Weight = 50
        },
        new Opponent()
        {
            Name = "Baby Cthulhu", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 300, Weakness = Weakness.None, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shift, 12) },
            GravityLevel = 10, Weight = 60
        },
        new Opponent()
        {
            Name = "Impostor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 50, Weakness = Weakness.None, Immunity = Immunity.xLines, XLineImmunity = 1, Cooldown = 6,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 1, 2) },
            GravityLevel = 3, Weight = 60
        },
        new Opponent()
        {
            Name = "PHILL", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 999999, Weakness = Weakness.xLines, XLineWeakness = 1, DamagesOnWeakness = 100000, Cooldown = 7,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Intoxication, 6) },
            GravityLevel = 6, Weight = 60,
            DialogId = 4, DialogPitch = 1.1f
        },
        new Opponent()
        {
            Name = "Truth", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 1, Weakness = Weakness.None, Cooldown = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 4, 1) },
            GravityLevel = 10, Weight = 60
        },
        new Opponent()
        {
            Name = "Boom Slayer", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.None, Cooldown = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 0) },
            GravityLevel = 5, Weight = 60,
            DialogId = 2, DialogPitch = 0.3f
        },
        new Opponent()
        {
            Name = "Baphomeh", Realm = Realm.Hell, Type = OpponentType.Boss,
            HpMax = 333, Weakness = Weakness.None, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drone, 2, 1) },
            GravityLevel = 6, Weight = 150,
            DialogId = 2, DialogPitch = 0.75f
        },
        new Opponent()
        {
            Name = "Devil's Advocate", Realm = Realm.Earth, Type = OpponentType.Boss,
            HpMax = 500, Weakness = Weakness.None, Cooldown = 4,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shift, 4) },
            GravityLevel = 10, Weight = 150,
            DialogId = 3, DialogPitch = 0.35f
        },
        new Opponent()
        {
            Name = "Stan", Realm = Realm.Hell, Type = OpponentType.Boss,
            HpMax = 666, Weakness = Weakness.None, Immunity = Immunity.xLines, XLineImmunity = 4, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Gate, 3) },
            GravityLevel = 10, Weight = 150,
            DialogId = 1, DialogPitch = 0.75f
        }
    };

    public static List<Opponent> EarthOpponents = new List<Opponent>();

    public static List<Opponent> HeavenOpponents = new List<Opponent>();
}
