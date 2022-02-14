using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OpponentsData
{
    //DEBUG
    public static bool DebugEnabled = Constants.OpponentsDebug;
    public static bool OnlyOpponent = false;
    public static Realm DebugRealm;
    public static Opponent DebugOpponent()
    {
        var name = "Shop Keeper";
        var tmpOpponent = HellOpponents?.Find(o => o.Name.Contains(name));
        DebugRealm = Realm.Hell;
        if (tmpOpponent == null)
        {
            tmpOpponent = EarthOpponents?.Find(o => o.Name.Contains(name));
            DebugRealm = Realm.Earth;
        }
        if (tmpOpponent == null)
        {
            tmpOpponent = HeavenOpponents?.Find(o => o.Name.Contains(name));
            DebugRealm = Realm.Heaven;
        }
        if (tmpOpponent == null)
            return null;
        return tmpOpponent.Clone(); ;
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
            HpMax = 222, Weakness = Weakness.None, Cooldown = 7,
            Attacks = new List<OpponentAttack>() {
            new OpponentAttack(AttackType.DarkRow, 2),
            new OpponentAttack(AttackType.WasteRow, 2, 1)},
            GravityLevel = 2, Weight = 0,
            Lore = "Youngest of the dummies, he wishes one day to take the place of his older brother, and thereby manage the training of hell's forces."
        },
        new Opponent()
        {
            Name = "Dark Dummy", Realm = Realm.Hell, Type = OpponentType.Elite,
            HpMax = 444, Weakness = Weakness.Combos, DamageOnWeakness = 6, Cooldown = 12,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 4, 6),
                new OpponentAttack(AttackType.ForcedPiece, 0, 1),
                new OpponentAttack(AttackType.EmptyRow, 2),
                new OpponentAttack(AttackType.ForcedPiece, -1, 1)},
            GravityLevel = 4, Weight = 0,
            Lore = "Older children of the dummies, his will is to honor the family duty, and prevent his little brother to murder him in order to take his place."
        },
        new Opponent()
        {
            Name = "Mother Dummy", Realm = Realm.Hell, Type = OpponentType.Champion,
            HpMax = 666, Weakness = Weakness.xLines, XLineWeakness = 4, DamageOnWeakness = 30, Cooldown = 10,
            Immunity = Immunity.xLines, XLineImmunity = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 3),
                new OpponentAttack(AttackType.WasteRow, 3, 3),
                new OpponentAttack(AttackType.EmptyRow, 3),
                new OpponentAttack(AttackType.LightRow, 3, 20)},
            GravityLevel = 8, Weight = 0,
            Lore = "Matriarch of the dummy family. Her reign over the hell's forces training is one of a kind. It's said she even trained the Antichrist himself!"
        },
        new Opponent()
        {
            Name = "Lost Soul", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 40, Weakness = Weakness.None, Cooldown = 10, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 1) },
            GravityLevel = 1, Weight = 10,
            Lore = "Soul of a lost undead."
        },
        new Opponent()
        {
            Name = "Ghoul", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 30, Weakness = Weakness.xLines, XLineWeakness = 2, DamageOnWeakness = 20, Cooldown = 8, Immunity = Immunity.xLines, XLineImmunity = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 1) },
            GravityLevel = 2, Weight = 10,
            Lore = "Ancient human dismorphed by the torture he received in hell."
        },
        new Opponent()
        {
            Name = "Hell Hound", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 20, Weakness = Weakness.None, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.AirPiece, 1) },
            GravityLevel = 2, Weight = 20,
            Lore = "The bad boys from earth end up in hell, becoming demon dogs mostly known as Hell Hounds."
        },
        new Opponent()
        {
            Name = "Warrior Soul", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.Combos, DamageOnWeakness = 10, Cooldown = 14, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 2),
                new OpponentAttack(AttackType.Shrink, 2)},
            GravityLevel = 3, Weight = 20,
            Lore = "Soul of a mighty undead in a previous life."
        },
        new Opponent()
        {
            Name = "Reverse Centaur", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 60, Weakness = Weakness.Twists, DamageOnWeakness = 70, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.MirrorMirror, 2, 1) },
            GravityLevel = 3, Weight = 30,
            Lore = "Centaurs were pretty difficult being to be crafted. All the failed attempts ended up in Hell."
        },
        new Opponent()
        {
            Name = "Fire Cultist", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 80, Weakness = Weakness.Combos, DamageOnWeakness = 5, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 3, 5) },
            GravityLevel = 1, Weight = 30,
            Lore = "The main element in hell is fire. And it's not rare to find old cultists worshipping it."
        },
        new Opponent()
        {
            Name = "Headbanger", Realm = Realm.Hell, Type = OpponentType.Common, Haste = true,
            HpMax = 80, Weakness = Weakness.None, Cooldown = 7,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.RhythmMania, 4, 1),
                new OpponentAttack(AttackType.Intoxication, 4) },
            GravityLevel = 3, Weight = 30,
            Lore = "Metalheads often end up in Hell, keeping their passion for heavy music, and their love for long haircuts."
        },
        new Opponent()
        {
            Name = "Dark Coffee", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 60, Weakness = Weakness.Twists, DamageOnWeakness = 50, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.MirrorMirror, 2, 0) },
            GravityLevel = 3, Weight = 30,
            Lore = "A drink so dark it could only have dark desires."
        },
        new Opponent()
        {
            Name = "Bass Sinner", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 120, Weakness = Weakness.None, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Partition, 2, 1),
                new OpponentAttack(AttackType.Partition, 3, 1) },
            GravityLevel = 1, Weight = 40,
            Lore = "Bassists always go to Hell. This is all they deserve by being totally worthless musicians."
        },
        new Opponent()
        {
            Name = "Giant Spider", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 99, Weakness = Weakness.Consecutive, DamageOnWeakness = 20, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drone, 1, 2) },
            GravityLevel = 4, Weight = 40,
            Lore = "Little spiders go to heaven for being good household bug killers. However, giant ones are already used to hell temperature since they mostly live in Austria."
        },
        new Opponent()
        {
            Name = "Tax Collector", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.xLines, XLineWeakness = 2, DamageOnWeakness = 50, Cooldown = 16, Immunity = Immunity.xLines, XLineImmunity = 1,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 10, 5) },
            GravityLevel = 3, Weight = 40,
            Lore = "The most despicable kind of human ever stepping on earth."
        },
        new Opponent()
        {
            Name = "Hell Raider", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 140, Weakness = Weakness.Twists, DamageOnWeakness = 40, Cooldown = 12,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedPiece, -1, -1) },
            GravityLevel = 4, Weight = 40,
            Lore = "Furious old metalhead, often racist, with a grudge against pretty much everything and everyone."
        },
        new Opponent()
        {
            Name = "Cat", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 77, Cooldown = 7, Immunity = Immunity.Cooldown,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.EmptyRow, 2) },
            GravityLevel = 3, Weight = 40,
            Lore = "The greatest manipulator of all, the cat is a true hellish creature."
        },
        new Opponent()
        {
            Name = "Ghoul Rider", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 1, 4),
                new OpponentAttack(AttackType.EmptyRow, 4),
            },
            GravityLevel = 3, Weight = 40,
            Lore = "Ghoul Riders found a clever way to march through Hell without proper footwear. It's their only positive aspect."
        },
        new Opponent()
        {
            Name = "Harpy", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 170, Weakness = Weakness.Twists, DamageOnWeakness = 20, Cooldown = 3,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedPiece, -2, 0) },
            GravityLevel = 4, Weight = 40,
            DialogId = 4, DialogPitch = 1.3f,
            Lore = "Main soldiers on Edam, Harpies are unpredictable and ferocious creatures."
        },
        new Opponent()
        {
            Name = "Tormentor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.Combos, DamageOnWeakness = 5, Cooldown = 17, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 2, 2) },
            GravityLevel = 3, Weight = 40,
            Lore = "Tormentors are the demons used for torturing animals abusers. They are good guys usually."
        },
        new Opponent()
        {
            Name = "Iron Maid", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 180, Weakness = Weakness.Twists, DamageOnWeakness = 20, Cooldown = 20,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 3) },
            GravityLevel = 3, Weight = 40,
            Lore = "Iron Maids are devoted to sexually torture pedophiles and zoophiles. They make pretty good arse-juice for those interested."
        },
        new Opponent()
        {
            Name = "Snek Jailor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 2) },
            GravityLevel = 4, Weight = 40,
            Lore = "Guardians of the imprisoned souls, Snek Jailors were once falsely accused prisoners, taking revenge over their fates."
        },
        new Opponent()
        {
            Name = "Crossed Demon", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.None, Cooldown = 18, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 3, 1) },
            GravityLevel = 1, Weight = 40,
            Lore = "Nobody knows where they come from as they can't hear, speak or use any form of sign language. But they are dope as fuck!"
        },
        new Opponent()
        {
            Name = "Lava Mole", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.Combos, DamageOnWeakness = 5, Cooldown = 6,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Tunnel, 3) },
            GravityLevel = 4, Weight = 40,
            Lore = "Oldest beings of Hell, Lava Moles were here even before the overlord settled down. Their knowledge of Hell's network is fantastic!"
        },
        new Opponent()
        {
            Name = "Bad Moose", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.xLines, XLineWeakness = 2, DamageOnWeakness = 20, Cooldown = 9,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LightRow, 2, 10) },
            GravityLevel = 1, Weight = 30,
            Lore = "Moose are bad."
        },
        new Opponent()
        {
            Name = "Caca Demon", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 120, Weakness = Weakness.Consecutive, DamageOnWeakness = 20, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedPiece, 3, 0) },
            GravityLevel = 5, Weight = 50,
            Lore = "Strangely enough, Caca Demons bleed red when hit, yet bleed blue when killed."
        },
        new Opponent()
        {
            Name = "Shop Keeper", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 14,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.AirPiece, 3) },
            GravityLevel = 5, Weight = 50,
            Lore = "Very talkative, he won't let you in peace until you've done \"The Thing\" with him."
        },
        new Opponent()
        {
            Name = "Baby Cthulhu", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 300, Weakness = Weakness.None, Cooldown = 12,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shift, 12) },
            GravityLevel = 8, Weight = 60,
            Lore = "Younger version of a dystopian god from another realm, you can feel happy not to have encounter his adult form."
        },
        new Opponent()
        {
            Name = "Impostor", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 50, Weakness = Weakness.None, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 1, 2) },
            GravityLevel = 3, Weight = 60,
            Lore = "He's the killer."
        },
        new Opponent()
        {
            Name = "PHILL", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 999999, Weakness = Weakness.xLines, XLineWeakness = 1, DamageOnWeakness = 100000, Cooldown = 9,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Intoxication, 6) },
            GravityLevel = 5, Weight = 60,
            DialogId = 4, DialogPitch = 1.1f,
            Lore = "Created from pure plagiarism, PHILL is a powerful being trying to get back to his reality."
        },
        new Opponent()
        {
            Name = "Truth", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 1, Weakness = Weakness.None, Cooldown = 3,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 4, 1) },
            GravityLevel = 8, Weight = 60,
            Lore = "The incarnation of truth itself, banished from Heaven, ended up in Hell for being considered as problematic."
        },
        new Opponent()
        {
            Name = "Boom Slayer", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.None, Cooldown = 3,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 0) },
            GravityLevel = 4, Weight = 60,
            DialogId = 2, DialogPitch = 0.3f,
            Lore = "As a new demonic threat erupts on Earth, so does the Slayer."
        },
        new Opponent()
        {
            Name = "Fallen Angel", Realm = Realm.Heaven, Type = OpponentType.Common, Haste = true,
            HpMax = 200, Weakness = Weakness.Consecutive, DamageOnWeakness = 40, Cooldown = 9,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LineBreak, 5) },
            GravityLevel = 5, Weight = 60,
            DialogId = 5, DialogPitch = 0.85f,
            Lore = "Celestial being, casted away from Heaven, but still preaching the word of his lord."
        },
        new Opponent()
        {
            Name = "Heaven's reject", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 170, Weakness = Weakness.None, Cooldown = 7,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LineBreak, 2) },
            GravityLevel = 4, Weight = 40,
            DialogId = 5, DialogPitch = 1.75f,
            Lore = "One of many Heaven's rejects, due to their imperfections not fitting for holy standards."
        },
        new Opponent()
        {
            Name = "Baphomeh", Realm = Realm.Hell, Type = OpponentType.Boss,
            HpMax = 444, Weakness = Weakness.None, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drone, 2, 1) },
            GravityLevel = 5, Weight = 999,
            DialogId = 2, DialogPitch = 0.75f,
            Lore = "Guardian of the gates of hell, Baphomeh curse bind him to the gates until eternity."
        },
        new Opponent()
        {
            Name = "Devil's Advocate", Realm = Realm.Earth, Type = OpponentType.Boss,
            HpMax = 444, Weakness = Weakness.None, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shift, 4),
                new OpponentAttack(AttackType.Shift, 2)},
            GravityLevel = 9, Weight = 999,
            DialogId = 3, DialogPitch = 0.35f,
            Lore = "Highest ranked magistrat down here, The Devil's Advocate endure the hard task of soul sorting and files handling."
        },
        new Opponent()
        {
            Name = "Stan", Realm = Realm.Hell, Type = OpponentType.Boss,
            HpMax = 666, Weakness = Weakness.None, Immunity = Immunity.xLines, XLineImmunity = 4, Cooldown = 12,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Gate, 3) },
            GravityLevel = 8, Weight = 999,
            DialogId = 1, DialogPitch = 0.75f,
            Lore = "FIrst fallen angel in history, Stan was bound to Hell as the overlord by force. Nevertheless, he found passion in his labor, and intended to do it right."
        }
    };

    public static List<Opponent> EarthOpponents = new List<Opponent>(){
        new Opponent()
        {
            Name = "Beginner monk", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.None, Cooldown = 10, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 2, 3) },
            GravityLevel = 4, Weight = 20,
            Lore = "A monk, in his first years of learning."
        },
        new Opponent()
        {
            Name = "Monk", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 6,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.EmptyRow, 2),
                new OpponentAttack(AttackType.DarkRow, 2) },
            GravityLevel = 6, Weight = 20,
            Lore = "Regular monk, despising everything that isn't holy."
        },
        new Opponent()
        {
            Name = "Nun", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 150, Weakness = Weakness.None, Cooldown = 8, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.MirrorMirror, 2, 2),
                new OpponentAttack(AttackType.Shift, 6) },
            GravityLevel = 6, Weight = 20,
            DialogId = 4, DialogPitch = 1.75f,
            Lore = "Regular Nun, despising everything that isn't chast."
        },
        new Opponent()
        {
            Name = "Living bible", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 30, Weakness = Weakness.None, Cooldown = 7, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shrink, 2) },
            GravityLevel = 8, Weight = 10,
            Lore = "What the fuck is that thing?"
        },
        new Opponent()
        {
            Name = "Punished Sinner", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.xLines, XLineWeakness = 4, DamageOnWeakness = 30, Cooldown = 6, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 1, 5) },
            GravityLevel = 8, Weight = 30,
            Lore = "Humans committing sins sometimes end up crucified. Which doesn't mean they cannot fight alongside the church afterward."
        },
        new Opponent()
        {
            Name = "Chorists", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 120, Weakness = Weakness.Combos, DamageOnWeakness = 10, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Partition, 5, 1) },
            GravityLevel = 8, Weight = 40,
            Lore = "Small group of traumatized children, singing traumatizing songs in Latin."
        },
        new Opponent()
        {
            Name = "Old Reverend", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 130, Weakness = Weakness.xLines, XLineWeakness = 4, DamageOnWeakness = 20, Cooldown = 11, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.OldSchool, 5, 10) },
            GravityLevel = 9, Weight = 40,
            Lore = "Back in his days, stackers didn't have Hard Drops or Hold options, and he thinks it was way better this way."
        },
        new Opponent()
        {
            Name = "Crusader", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 250, Weakness = Weakness.xLines, XLineWeakness = 4, DamageOnWeakness = 20, Cooldown = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 4, 1) },
            GravityLevel = 9, Weight = 40,
            Lore = "Enlightened knight thinking that reconquering holy lands will change anything."
        },
        new Opponent()
        {
            Name = "Flag Wielder", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 100, Weakness = Weakness.None, Immunity = Immunity.xLines, XLineImmunity = 3, Cooldown = 10, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Screwed, 2) },
            GravityLevel = 6, Weight = 30,
            Lore = "Flag Wielders must know how to speak in heraldic, and follow their designated knight singing his praise."
        },
        new Opponent()
        {
            Name = "Human Supremacist", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.Twists, DamageOnWeakness = 70, Cooldown = 5, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 1, 1),
                new OpponentAttack(AttackType.AirPiece, 1),
                new OpponentAttack(AttackType.Screwed, 1)},
            GravityLevel = 9, Weight = 40,
            DialogId = 2, DialogPitch = 0.4f,
            Lore = "Strongest soldiers of the 40th millenium, they also come by the name of Shpashe Merines."
        },
        new Opponent()
        {
            Name = "Lost Cherub", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 120, Cooldown = 7, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DropBomb, 10) },
            GravityLevel = 5, Weight = 30,
            Lore = "Smallest of the angels, this Cherub seems to have lost his way on earth."
        },
        new Opponent()
        {
            Name = "Angelic Messenger", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 170, Cooldown = 8, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LineBreak, 5),
                new OpponentAttack(AttackType.DropBomb, 6) },
            GravityLevel = 8, Weight = 40,
            DialogId = 5, DialogPitch = 0.5f,
            Lore = "The speech of the Lord has only few times in history been told to humans by the Lord himself. Most of the time, Messengers are used for this purpose."
        },
        new Opponent()
        {
            Name = "Cheese Cake", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 130, Weakness = Weakness.Twists, DamageOnWeakness = 150, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 2),
                new OpponentAttack(AttackType.VisionBlock, 10, 5)},
            GravityLevel = 8, Weight = 20,
            Lore = "Is it cheese? Is it cake? We will never know!"
        },
        new Opponent()
        {
            Name = "Organist", Realm = Realm.Earth, Type = OpponentType.Common, Haste = true,
            HpMax = 150, Weakness = Weakness.xLines, XLineWeakness = 4, DamageOnWeakness = 20, Cooldown = 12,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.RhythmMania, 10, 1)},
            GravityLevel = 8, Weight = 40,
            Lore = "The Organ is a pretty easy instrument to learn. Nothing particular to say about the people playing it."
        },
        new Opponent()
        {
            Name = "Karen", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 200, Immunity = Immunity.xLines, XLineImmunity = 3, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drone, 4, 0) },
            GravityLevel = 6, Weight = 60,
            Lore = "One of the most despicable humans on earth. Run if she asks for the manager!"
        },
        new Opponent()
        {
            Name = "Stained Glass", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 90, Weakness = Weakness.xLines, XLineWeakness = 4, DamageOnWeakness = 60, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 4, 1) },
            GravityLevel = 6, Weight = 40,
            Lore = "They can fight apparently... I'm the first stunned by it!"
        },
        new Opponent()
        {
            Name = "Wololo", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 100, Immunity = Immunity.xLines, XLineImmunity = 1, Cooldown = 6,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shrink, 1),
                new OpponentAttack(AttackType.OldSchool, 4, 8) },
            GravityLevel = 5, Weight = 40,
            Lore = "Don't let them get you into an discussion, or you'll soon be manipulated into joining their ranks!"
        },
        new Opponent()
        {
            Name = "Politician", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.Twists, DamageOnWeakness = 50, Cooldown = 8,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 2) },
            GravityLevel = 9, Weight = 40,
            Lore = "Legends say some good ones exist. Legends..."
        },
        new Opponent()
        {
            Name = "Vampire Killer", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 500, Weakness = Weakness.Twists, DamageOnWeakness = 200, Cooldown = 4,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 2),
                new OpponentAttack(AttackType.EmptyRow, 2) },
            GravityLevel = 8, Weight = 40,
            DialogId = 4, DialogPitch = 0.75f,
            Lore = "Often armed with a whip, Vampire Slayers have best first levels musics, whatever it means!"
        },
        new Opponent()
        {
            Name = "Witch Hunter", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 500, Weakness = Weakness.xLines, XLineWeakness = 4, DamageOnWeakness = 200, Cooldown = 8, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.DarkRow, 2) },
            GravityLevel = 8, Weight = 40,
            DialogId = 3, DialogPitch = 0.75f,
            Lore = "Old hunters liking old traditions and sick of hunting animals. Yes it is just a pretext to kill women."
        },
        new Opponent()
        {
            Name = "Exorcist", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 150, Immunity = Immunity.xLines, XLineImmunity = 4, Cooldown = 6, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Intoxication, 5),
                new OpponentAttack(AttackType.WasteRow, 2, 2) },
            GravityLevel = 8, Weight = 30,
            DialogId = 3, DialogPitch = 0.5f,
            Lore = "Directly sent from the Vatican, they represent the highest rank of the church to deal with paranormal and demons."
        },
        new Opponent()
        {
            Name = "Kultist", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 180, Cooldown = 6, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LightRow, 2, 5),
                new OpponentAttack(AttackType.LightRow, 4, 5),
                new OpponentAttack(AttackType.Shrink, 1)
            },
            GravityLevel = 6, Weight = 40,
            Lore = "Your good old radical racist kultist from Texas."
        },
        new Opponent()
        {
            Name = "Matriarch", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 200, Immunity = Immunity.Twists, Weakness = Weakness.Combos, DamageOnWeakness = 15, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Gate, 1) },
            GravityLevel = 9, Weight = 40,
            DialogId = 4, DialogPitch = 0.75f,
            Lore = "Their soul is so dark, it is hard to believe they belongs in the church..."
        },
        new Opponent()
        {
            Name = "Worm Ghast", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 200, Immunity = Immunity.xLines, XLineImmunity = 4, Weakness = Weakness.Consecutive, DamageOnWeakness = 50, Cooldown = 5,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.AirPiece, 3),
                new OpponentAttack(AttackType.Tunnel, 2)},
            GravityLevel = 5, Weight = 25,
            Lore = "Lowest form of ghosts, those strange being are nevertheless working with the men of faith."
        },
        new Opponent()
        {
            Name = "Igorr", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 600, Weakness = Weakness.Combos, DamageOnWeakness = 20, Cooldown = 22,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 6, 10) },
            GravityLevel = 8, Weight = 40,
            Lore = "Church doesn't only have pedophiles, they also have pedovores."
        },
        new Opponent()
        {
            Name = "Mirror of Erised", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 200, Weakness = Weakness.Combos, DamageOnWeakness = 20, Cooldown = 17, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.MirrorMirror, 4, 1) },
            GravityLevel = 2, Weight = 40,
            Lore = "This old device shows your deepest desires!"
        },
        new Opponent()
        {
            Name = "Tactical Nun", Realm = Realm.Earth, Type = OpponentType.Boss,
            HpMax = 600, Weakness = Weakness.None, Cooldown = 4,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Gate, 1),
                new OpponentAttack(AttackType.Shift, 40),
                new OpponentAttack(AttackType.Drill, 1),
                new OpponentAttack(AttackType.Screwed, 1) },
            GravityLevel = 12, Weight = 999,
            DialogId = 4, DialogPitch = 1.25f, HeadDown = true,
            Lore = "Ancient agent of a shady organization, this professional killer devoted her life to the church to repent her sins."
        },
        new Opponent()
        {
            Name = "Supreme Bishop", Realm = Realm.Earth, Type = OpponentType.Boss,
            HpMax = 600, Weakness = Weakness.Combos, DamageOnWeakness = 10, Cooldown = 12, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 2, 8) },
            GravityLevel = 9, Weight = 999,
            DialogId = 0, DialogPitch = 1.0f,
            Lore = "One of the oldest member of the clergy, this immortal being tend to still do thing the old way..."
        },
        new Opponent()
        {
            Name = "The Pop", Realm = Realm.Heaven, Type = OpponentType.Boss,
            HpMax = 1000, Weakness = Weakness.Combos, DamageOnWeakness = 10, Immunity = Immunity.Twists, Cooldown = 16, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.WasteRow, 6, 2)},
            GravityLevel = 8, Weight = 999,
            DialogId = 2, DialogPitch = 1.1f,
            Lore = "Definitely a human, who was definitely chosen by the Lord and no one else to reign over humans and definitely not lizard people."
        }
    };

    public static List<Opponent> HeavenOpponents = new List<Opponent>()
    {
        new Opponent()
        {
            Name = "Watcher", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 200, Cooldown = 9, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.RhythmMania, 4, 1) },
            GravityLevel = 9, Weight = 20,
            Lore = "Few is known of watchers, as they can be totally invisible until confrontation, as humans think anyway. But they spy humans that's for sure."
        },
        new Opponent()
        {
            Name = "Winged Liberty", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 200, Cooldown = 6, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shelter, 1),
                new OpponentAttack(AttackType.LightRow, 4, 10),
                new OpponentAttack(AttackType.Shelter, 1),
                new OpponentAttack(AttackType.EmptyRow, 5) },
            GravityLevel = 9, Weight = 20,
            Lore = "Symbol of the liberty, of the fight against oppression, it however was never sent to humans..."
        },
        new Opponent()
        {
            Name = "Shield of Light", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 400, Cooldown = 6, Haste = true, Immunity = Immunity.xLines, XLineImmunity = 3,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Shelter, 1),
                new OpponentAttack(AttackType.Shift, 4) },
            GravityLevel = 9, Weight = 20,
            Lore = "Protectors of Heaven, those mighty shields can block even the heaviest attacks!"
        },
        new Opponent()
        {
            Name = "Truthful Truth", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 250, Cooldown = 9, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.ForcedBlock, 1, 2) },
            GravityLevel = 11, Weight = 30,
            Lore = "When you think you reached the truth, this truth will hit you with a truth, truther that the truth you trusted."
        },
        new Opponent()
        {
            Name = "Lower Throne", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 300, Cooldown = 9, Haste = true, Immunity = Immunity.Twists,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LineBreak, 3) },
            GravityLevel = 12, Weight = 30,
            Lore = "Thrones are ancient angels, they were here when the earth was created, and did nothing to prevent the human to tear apart each others."
        },
        new Opponent()
        {
            Name = "Cherub", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 250, Cooldown = 10, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LineBreak, 6),
                new OpponentAttack(AttackType.DropBomb, 5) },
            GravityLevel = 8, Weight = 30,
            Lore = "Smallest of the angels, look how cute they are, with their little wings and their little child skull."
        },
        new Opponent()
        {
            Name = "Enchant", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 300, Cooldown = 6, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drone, 2, 0),
                new OpponentAttack(AttackType.VisionBlock, 4, 2),
                new OpponentAttack(AttackType.Drone, 2, 2),
                new OpponentAttack(AttackType.VisionBlock, 4, 2),
             },
            GravityLevel = 11, Weight = 40,
            Lore = "Tortured angel, taking the weight of human sins on his shoulders."
        },
        new Opponent()
        {
            Name = "Angel", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 400, Cooldown = 10, Haste = true, Weakness = Weakness.xLines, XLineWeakness = 4, DamageOnWeakness = 100,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LineBreak, 4),
                new OpponentAttack(AttackType.Ascension, 4, 2)},
            GravityLevel = 12, Weight = 40,
            Lore = "Basic angel. As unusefull and not caring as any angel can be."
        },
        new Opponent()
        {
            Name = "Kinky Angel", Realm = Realm.Heaven, Type = OpponentType.Common,
            HpMax = 350, Cooldown = 9, Haste = true, Weakness = Weakness.Combos, DamageOnWeakness = 10,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Drill, 4),
                new OpponentAttack(AttackType.Gate, 2) },
            GravityLevel = 12, Weight = 40,
            Lore = "Still wondering how this angel didn't end up in Hell... It shows the Heaven's chastity hypocrysis."
        },
        new Opponent()
        {
            Name = "Duality Angel", Realm = Realm.Hell, Type = OpponentType.Common,
            HpMax = 300, Cooldown = 9, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Ascension, 1, 3),
                new OpponentAttack(AttackType.Tunnel, 4),
                new OpponentAttack(AttackType.Shrink, 1) },
            GravityLevel = 12, Weight = 40,
            Lore = "Representing a neutral opinion among angels, his devious side is often bribed in order to satisfy most of the angels prerogatives."
        },
        new Opponent()
        {
            Name = "Harambe", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 500, Cooldown = 13, Haste = true, Weakness = Weakness.Combos, DamageOnWeakness = 20,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Ascension, 7, 1) },
            GravityLevel = 12, Weight = 50,
            DialogId = 2, DialogPitch = 0.35f,
            Lore = "The one and only. He died for our sins."
        },
        new Opponent()
        {
            Name = "Prophet Raptor", Realm = Realm.Earth, Type = OpponentType.Common,
            HpMax = 350, Cooldown = 8, Haste = true, Immunity = Immunity.Cooldown,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.VisionBlock, 5, 2),
                new OpponentAttack(AttackType.Screwed, 1) },
            GravityLevel = 12, Weight = 50,
            Lore = "The true aspect of the prophet is hard to accept, but is reachable for the chosen ones!"
        },
        new Opponent()
        {
            Name = "Seraphim King", Realm = Realm.Heaven, Type = OpponentType.Boss,
            HpMax = 1111, Weakness = Weakness.xLines, XLineWeakness = 4, DamageOnWeakness = 100, Cooldown = 8, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LineBreak, 6),
                new OpponentAttack(AttackType.Drone, 2, 0) },
            GravityLevel = 14, Weight = 999,
            DialogId = 5, DialogPitch = 0.35f,
            Lore = "King of Kings, this old seraphim reign over angels of Heaven with an iron fist!"
        },
        new Opponent()
        {
            Name = "Divine Council", Realm = Realm.Heaven, Type = OpponentType.Boss,
            HpMax = 1111, Weakness = Weakness.Combos, DamageOnWeakness = 50, Cooldown = 8, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.Gate, 2),
                new OpponentAttack(AttackType.OldSchool, 4, 10) },
            GravityLevel = 14, Weight = 999,
            DialogId = 5, DialogPitch = 0.2f,
            Lore = "Oldest entities existing, eternal beings, having always existed, even before the big bang. They elected the Lord of Heaven."
        }, 
        new Opponent()
        {
            Name = "DOG", Realm = Realm.Heaven, Type = OpponentType.Boss,
            HpMax = 2222, Weakness = Weakness.Twists, DamageOnWeakness = 100, Immunity = Immunity.Combos, Cooldown = 8, Haste = true,
            Attacks = new List<OpponentAttack>() {
                new OpponentAttack(AttackType.LineBreak, 8),
                new OpponentAttack(AttackType.Shift, 4),
                new OpponentAttack(AttackType.Ascension, 4, 2) },
            GravityLevel = 15, Weight = 999,
            DialogId = 5, DialogPitch = 1.0f,
            Lore = "God of Gods, ruler of Heaven, good boy, and put on the throne by questionable manners..."
        }
    };
}
