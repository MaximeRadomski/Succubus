using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharactersData
{
    public static List<Character> Characters = new List<Character>()
    {
        //HELL
        new Character()
        {
            Id = 0, Name = "Ivy", Kind = "Succubus", Realm = Realm.Hell,
            Attack = 10,
            Cooldown = 10,
            SpecialName = "Whip Hits", SpecialDescription = "transform the current and next 2 pieces into a single cube"
        },
        new Character()
        {
            Id = 1, Name = "Odamm", Kind = "Incubus", Realm = Realm.Hell,
            Attack = 10,
            Cooldown = 10,
            SpecialName = "Sin's Weight", SpecialDescription = "blocks of the current and next piece are affected by gravity"
        },
        new Character()
        {
            Id = 2, Name = "Podarge", Kind = "Harpy", Realm = Realm.Hell,
            Attack = 8,
            Cooldown = 5,
            SpecialName = "Mine", SpecialDescription = "switch current piece with the last of the piece preview"
        },
        new Character()
        {
            Id = 3, Name = "Belias", Kind = "Slave Demon", Realm = Realm.Hell,
            Attack = 12,
            Cooldown = 20,
            SpecialName = "Again", SpecialDescription = "replace the next 3 pieces by the current one"
        },
        //EARTH
        new Character()
        {
            Id = 4, Name = "Floppyredoux", Kind = "Witch", Realm = Realm.Earth,
            Attack = 12,
            Cooldown = 15,
            SpecialName = "Time Drop", SpecialDescription = "the next 5 pieces aren't affected by gravity"
        },
        new Character()
        {
            Id = 5, Name = "Sir Vixid", Kind = "Dark Templar", Realm = Realm.Earth,
            Attack = 12,
            Cooldown = 20,
            SpecialName = "Diabolus Vult", SpecialDescription = "remove the last 4 rows from the top"
        },
        new Character()
        {
            Id = 6, Name = "Theresa", Kind = "Perverted Nun", Realm = Realm.Earth,
            Attack = 8,
            Cooldown = 8,
            SpecialName = "Penitence", SpecialDescription = "remove the last 2 rows from the top for a grey one"
        },
        new Character()
        {
            Id = 7, Name = "Jean", Kind = "Corrupted Monk", Realm = Realm.Earth,
            Attack = 8,
            Cooldown = 20,
            SpecialName = "Resilience", SpecialDescription = "cancel next enemy attack"
        },
        //HEAVEN
        new Character()
        {
            Id = 8, Name = "Melip", Kind = "Seraphim", Realm = Realm.Heaven,
            Attack = 6,
            Cooldown = 7,
            SpecialName = "Pride", SpecialDescription = "randomize all piece preview"
        },
        new Character()
        {
            Id = 9, Name = "Enepsigos", Kind = "Fallen Angel", Realm = Realm.Heaven,
            Attack = 10,
            Cooldown = 10,
            SpecialName = "Wisdom", SpecialDescription = "peplenish 10% of enemy HP but remove two grey rows from the top ones"
        },
        new Character()
        {
            Id = 10, Name = "Azrael", Kind = "Seduced Archangel", Realm = Realm.Heaven,
            Attack = 12,
            Cooldown = 30,
            SpecialName = "Punishment", SpecialDescription = "remove one random half of the collumns"
        },
        new Character()
        {
            Id = 11, Name = "Clarity", Kind = "Divine Betrayer", Realm = Realm.Heaven,
            Attack = 14,
            Cooldown = 40,
            SpecialName = "Sky Down", SpecialDescription = "apply gravity on all current blocks in the play field (no bonuses)"
        },
    };
}
