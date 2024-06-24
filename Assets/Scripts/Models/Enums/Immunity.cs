using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Immunity
{
    public Immunity(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public int Id;
    public string Name;
    public string Description;

    public static Immunity FromId(int id)
    {
        return Immunities.FirstOrDefault(t => t.Id == id);
    }

    public override string ToString()
    {
        return Name.Replace(" ", "");
    }

    public static Immunity None = new Immunity(0, "None", "None");
    public static Immunity Combos = new Immunity(1, "Combos", "this opponent won't receive damage from combos.");
    public static Immunity xLines = new Immunity(2, "x Lines", "this opponent won't receive damage from x lines.");
    public static Immunity Twists = new Immunity(3, "Twists", "this opponent won't receive damage from twists.");
    public static Immunity Consecutive = new Immunity(4, "CC Lines", "this opponent won't receive damage damage from consecutive lines.");
    public static Immunity Cooldown = new Immunity(5, "Fixed Cooldown", "this opponent won't get its cooldown altered.");

    public static List<Immunity> Immunities = new List<Immunity>() {
        Combos,
        xLines,
        Twists,
        Consecutive,
        Cooldown
    };
}

public enum ImmunityEnum
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
    [Description("Fixed Cooldown")]
    Cooldown = 5,
}
