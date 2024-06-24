using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Weakness
{
    public Weakness(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public int Id;
    public string Name;
    public string Description;

    public static Weakness FromId(int id)
    {
        return Weaknesses.FirstOrDefault(t => t.Id == id);
    }

    public override string ToString()
    {
        return Name.Replace(" ", "");
    }

    public static Weakness None = new Weakness(0, "None", "None");
    public static Weakness Combos = new Weakness(1, "Combos", "this opponent receives more damage from combos.");
    public static Weakness xLines = new Weakness(2, "x Lines", "this opponent receives more damage from x lines.");
    public static Weakness Twists = new Weakness(3, "Twists", "this opponent receives damage from twists.");
    public static Weakness Consecutive = new Weakness(4, "CC Lines", "this opponent receives more damage from consecutive lines.");

    public static List<Weakness> Weaknesses = new List<Weakness>() {
        Combos,
        xLines,
        Twists,
        Consecutive
    };
}

public enum WeaknessEnum
{
    [Description("None")]
    None = 0,
    [Description("Combos")]
    Combos = 1,
    [Description(" Lines")]
    xLines = 2,
    [Description("Twists")]
    Twists = 3,
    [Description("CC Lines")]
    Consecutive = 4,
}
