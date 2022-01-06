using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharactersData
{
    //DEBUG
    public static bool DebugEnabled = Constants.CharactersDebug;
    public static Character DebugCharacter()
    {
        return Characters?.Find(o => o.Name.Contains("Tony"));
    }

    static CharactersData()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Characters[i].StartingRealm = Characters[i].Realm;
        }
    }

    public static List<Character> Characters = new List<Character>()
    {
        //HELL
        new Character()
        {
            Id = 0, Name = "Ivy", Kind = "Succubus", Realm = Realm.Hell,
            Attack = 8,
            Cooldown = 10,
            SpecialName = "Whip Hits", SpecialDescription = "summons 3 single cubes as your next pieces.",
            Lore = "Daughter of lust itself, she makes anyone her slave under the yoke of her whip. Perpetual torment and pleasure are her doom!",
            DialogId = 3, DialogPitch = 1.5f
        },
        new Character()
        {
            Id = 1, Name = "Edam", Kind = "Incubus", Realm = Realm.Hell,
            Attack = 8,
            Cooldown = 8,
            SpecialName = "Sin's Weight", SpecialDescription = "the blocks of the next 6 pieces are individually affected by gravity.",
            Lore = "Ivy's less exuberant twin brother. He possesses dark needs which cannot be fulfilled. His army of harpies constantly hunts down hell looking for his next victims.",
            DialogId = 3, DialogPitch = 0.85f
        },
        new Character()
        {
            Id = 2, Name = "Podarge", Kind = "Harpy", Realm = Realm.Hell,
            Attack = 6,
            Cooldown = 6,
            SpecialName = "Swap", SpecialDescription = "switches the current piece with one of the piece preview.",
            Lore = "Captain of the harpies army, her eternal past of trickery and ferocity gave her enough power in order to command a troop of monsters whom are considered untamable !",
            DialogId = 4, DialogPitch = 1.8f
        },
        new Character()
        {
            Id = 3, Name = "Belias", Kind = "Slave Demon", Realm = Realm.Hell,
            Attack = 10,
            Cooldown = 14,
            SpecialName = "Hollowed", SpecialDescription = "hollows your current piece, allowing it to go through blocks.",
            Lore = "Once one of the hell's gatekeepers, he fell in adoration for Ivy. Many succumbed under the terrific strength of his fists. He now endlessly serves his new mistress as a bodyguard.",
            DialogId = 1, DialogPitch = 0.5f
        },
        //EARTH
        new Character()
        {
            Id = 4, Name = "Floppyredoux", Kind = "Witch", Realm = Realm.Earth,
            Attack = 7,
            Cooldown = 7,
            SpecialName = "Oketi Poketi", SpecialDescription = "the next 7 pieces have one less block.",
            Lore = "Exiled for being an emancipated woman and a witch, Floppy never stopped helping the ones in need, even if it meant making them drink weird flasks.",
            DialogId = 3, DialogPitch = 1.0f
        },
        new Character()
        {
            Id = 5, Name = "Sir Vixid", Kind = "Dark Templar", Realm = Realm.Earth,
            Attack = 7,
            Cooldown = 11,
            SpecialName = "Diabolus Vult", SpecialDescription = "removes the last 4 rows from the top for 4 waste ones on the bottom.",
            Lore = "Last representant of a long dead religion, this hell crusader stood against holiness for centuries, fulfilling his pact in exchange of his immortality!",
            DialogId = 1, DialogPitch = 0.5f
        },
        new Character()
        {
            Id = 6, Name = "Cereza", Kind = "Perverted Nun", Realm = Realm.Earth,
            Attack = 6,
            Cooldown = 4,
            SpecialName = "Penitence", SpecialDescription = "cancels the last piece locked on the playfield.",
            Lore = "Lust, luxury, avarice, envy, nothing is bad enough for her. Her years in convent made her realise it felt good to be bad once in a while.",
            DialogId = 4, DialogPitch = 1.75f
        },
        new Character()
        {
            Id = 7, Name = "Tony", Kind = "Corrupted Monk", Realm = Realm.Earth,
            Attack = 6,
            Cooldown = 6,
            SpecialName = "Resilience", SpecialDescription = "cancels next enemy attack.",
            Lore = "Once patriarch of a brewer covenant, this old monk found Satan in his addiction to alcohol. He's still trying to forget it by drinking...",
            DialogId = 2, DialogPitch = 1.0f
        },
        //HEAVEN
        new Character()
        {
            Id = 8, Name = "Melip", Kind = "Cherub", Realm = Realm.Heaven,
            Attack = 6,
            Cooldown = 6,
            SpecialName = "Pride", SpecialDescription = "replaces your last 2 to 3 rows by a 2 to 3 T-twist pattern.",
            Lore = "Born malformed, Melip was quickly categorized as impure. He has lived with hatred from his kind, and swore one day to take revenge.",
            DialogId = 5, DialogPitch = 1.6f
        },
        new Character()
        {
            Id = 9, Name = "Azrael", Kind = "Seduced Archangel", Realm = Realm.Heaven,
            Attack = 12,
            Cooldown = 2,
            SpecialName = "Wisdom", SpecialDescription = "your next piece destroys any blocks it touches.",
            Lore = "Being the angel of death made him reconsider his education between life and death but also between good and bad. Azrael made up his mind, he doesn't want to be good anymore.",
            DialogId = 5, DialogPitch = 0.6f
        },
        new Character()
        {
            Id = 10, Name = "Uriel", Kind = "Seraphim", Realm = Realm.Heaven,
            Attack = 10,
            Cooldown = 16,
            SpecialName = "Punishment", SpecialDescription = "removes one random half of the columns.",
            Lore = "Banished from the skies after an argument about chasteness amongst angels, Uriel took the liberty to steal the Spear of Longinus in it's way out, granting him an immense power!",
            DialogId = 5, DialogPitch = 1.0f
        },
        new Character()
        {
            Id = 11, Name = "Zaphkiel", Kind = "Throne Betrayer", Realm = Realm.Heaven,
            Attack = 14,
            Cooldown = 20,
            SpecialName = "Sky Down", SpecialDescription = "applies gravity on all current blocks in the play field (no bonuses).",
            Lore = "Simple throne but hard worker, Zaphkiel made his way to the top of the Divine Council. But his dedication, considered as too dangerous for the power in place, put him into prison.",
            DialogId = 5, DialogPitch = 0.8f
        },
    };

    public static List<Character> CustomCharacters = new List<Character>()
    {
        new Character()
        {
            Id = 0, Name = "Old Man", Kind = "EarthOpponents_6", Realm = Realm.Earth,
            Attack = 0,
            Cooldown = 0,
            SpecialName = null, SpecialDescription = "None",
            Lore = "None",
            DialogId = 0, DialogPitch = 1.0f
        },
        new Character()
        {
            Id = 49, Name = "The Beholder", Kind = "Mystic Being", Realm = Realm.Heaven,
            Attack = 0,
            Cooldown = 0,
            SpecialName = null, SpecialDescription = "None",
            Lore = "None",
            DialogId = 4, DialogPitch = 0.5f
        }
    };
}
