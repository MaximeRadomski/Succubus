using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharactersData
{
    //DEBUG
    public static bool DebugEnabled = true;
    public static Character DebugCharacter()
    {
        return Characters?.Find(o => o.Name.Contains("Floppyredoux"));
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
            Cooldown = 8,
            SpecialName = "Oketi Poketi", SpecialDescription = "the next 7 pieces have one less block."
        },
        new Character()
        {
            Id = 5, Name = "Sir Vixid", Kind = "Dark Templar", Realm = Realm.Earth,
            Attack = 9,
            Cooldown = 12,
            SpecialName = "Diabolus Vult", SpecialDescription = "remove the last 4 rows from the top for 4 garbage ones."
        },
        new Character()
        {
            Id = 6, Name = "Theresa", Kind = "Perverted Nun", Realm = Realm.Earth,
            Attack = 5,
            Cooldown = 2,
            SpecialName = "Penitence", SpecialDescription = "cancel the last piece locked on the playfield."
        },
        new Character()
        {
            Id = 7, Name = "Tony", Kind = "Corrupted Monk", Realm = Realm.Earth,
            Attack = 5,
            Cooldown = 14,
            SpecialName = "Resilience", SpecialDescription = "cancel next enemy attack."
        },
        //HEAVEN
        new Character()
        {
            Id = 8, Name = "Melip", Kind = "Seraphim", Realm = Realm.Heaven,
            Attack = 6,
            Cooldown = 7,
            SpecialName = "Pride", SpecialDescription = "randomize all piece preview."
        },
        new Character()
        {
            Id = 9, Name = "Enepsigos", Kind = "Fallen Angel", Realm = Realm.Heaven,
            Attack = 10,
            Cooldown = 10,
            SpecialName = "Wisdom", SpecialDescription = "banish the current piece from existence."
        },
        new Character()
        {
            Id = 10, Name = "Azrael", Kind = "Seduced Archangel", Realm = Realm.Heaven,
            Attack = 12,
            Cooldown = 20,
            SpecialName = "Punishment", SpecialDescription = "remove one random half of the collumns."
        },
        new Character()
        {
            Id = 11, Name = "Clarity", Kind = "Divine Betrayer", Realm = Realm.Heaven,
            Attack = 14,
            Cooldown = 20,
            SpecialName = "Sky Down", SpecialDescription = "apply gravity on all current blocks in the play field (no bonuses)."
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
        }
    };
}
