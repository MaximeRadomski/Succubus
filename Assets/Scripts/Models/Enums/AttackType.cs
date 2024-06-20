using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using UnityEngine;

public class AttackType
{
    public AttackType(int id, string name, string description, string prefixe, string suffixe)
    {
        Id = id;
        Name = name;
        Description = description;
        Prefixe = prefixe;
        Suffixe = suffixe;
    }

    public int Id;
    public string Name;
    public string Description;
    public string Prefixe;
    public string Suffixe;

    public static AttackType FromId(int id)
    {
        return AttackTypes.FirstOrDefault(t => t.Id == id);
    }

    public override string ToString() {
        return Name.Replace(" ", "");
    }

    public static AttackType None = new AttackType(0, "None", "None", "", "");
    public static AttackType DarkRow = new AttackType(1, "Dark Row", "a row destroyed by clearing 2+ rows.", "x", "");
    public static AttackType WasteRow = new AttackType(2, "Waste Row", "a row with a hole in it.", "x", "");
    public static AttackType LightRow = new AttackType(3, "Light Row", "a row with a cooldown. locking a piece decreases it.", "x", "");
    public static AttackType EmptyRow = new AttackType(4, "Empty Row", "a row of nothing.", "x", "");
    public static AttackType VisionBlock = new AttackType(5, "Vision Block", "a block partially hiding your playfield.", "", " rows");
    public static AttackType ForcedPiece = new AttackType(6, "Forced Piece", "a piece added by force to your playfield.", "", "");
    public static AttackType Drill = new AttackType(7, "Drill", "drills a hole in your playfield.", "", "");
    public static AttackType AirPiece = new AttackType(8, "Air Piece", "an almost transparent piece.", "", " pieces");
    public static AttackType ForcedBlock = new AttackType(9, "Forced Block", "a block added to your piece.", "", " pieces");
    public static AttackType UpsideDown = new AttackType(10, "Upside Down", "reverses the camera through the x and/or y axis.", "", " pieces");
    public static AttackType Intoxication = new AttackType(11, "Intoxication", "makes you experience the effects of alcohol.", "", " pieces");
    public static AttackType Drone = new AttackType(12, "Drone", "invokes a drone that spawn lines until pounded.", "", "");
    public static AttackType Shift = new AttackType(13, "Shift", "shifts some rows of your playfield horizontally.", "", " rows");
    public static AttackType Gate = new AttackType(14, "Gate", "a row with a hole at the top of your playfield.", "", " pieces");
    public static AttackType SheetMusic = new AttackType(15, "Sheet Music", "a sheet of directional inputs.", "", " notes");
    public static AttackType Shrink = new AttackType(16, "Shrink", "shrinks your playfield from the bottom.", "", " lines");
    public static AttackType OldSchool = new AttackType(17, "Old School", "forces a retro gameplay.", "", " pieces");
    public static AttackType Screwed = new AttackType(18, "Old School", "prevent piece rotation.", "", " pieces");
    public static AttackType BombDrop = new AttackType(19, "Bomb Drop", "a cooldown of moves until hard dropping.", "", " moves");
    public static AttackType Tunnel = new AttackType(20, "Tunnel", "a vertical hole in your playfield.", "", " deep");
    public static AttackType RhythmMania = new AttackType(21, "Rhythm Mania", "a rhythm forcing you to play in beat.", "", " beats");
    public static AttackType LineBreak = new AttackType(22, "Line Break", "a limit that prevents line clearing until reached.", "", " lines");
    public static AttackType Shelter = new AttackType(23, "Shelter", "a shield that counters your next attack.", "", " attacks");
    public static AttackType Ascension = new AttackType(24, "Ascension", "rises a part of your playfield.", "", " columns");

    public static List<AttackType> AttackTypes = new List<AttackType>() {
        DarkRow,
        WasteRow,
        LightRow,
        EmptyRow,
        VisionBlock,
        ForcedPiece,
        Drill,
        AirPiece,
        ForcedBlock,
        UpsideDown,
        Intoxication,
        Drone,
        Shift,
        Gate,
        SheetMusic,
        Shrink,
        OldSchool,
        Screwed,
        BombDrop,
        Tunnel,
        RhythmMania,
        LineBreak,
        Shelter,
        Ascension
    };
}

public enum AttackTypeEnum
{
    //Param1 will be the one displayed between pefixes and suffixes

    None = 0,
    [Prefixe("x")]
    [Suffixe(null)]
    [Description("Dark Row")]
    DarkRow = 1, //Full rows, can only be destroyed by doing lines (2 clear 1, 3 clear 2, 4 clear 3) (param1) -> nb Lines
    [Prefixe("x")]
    [Suffixe(null)]
    [Description("Waste Row")]
    WasteRow = 2, //Standard rows with holes in it (param1) -> nb Lines, param2 -> nb holes
    [Prefixe("x")]
    [Suffixe(null)]
    [Description("Light Row")]
    LightRow = 3, //Full rows with a cooldown, each time a piece locks, it decreases the cooldown. Is destroyed after cooldown. (param1) nbRows, (param2) cooldown
    [Prefixe("x")]
    [Suffixe(null)]
    [Description("Empty Row")]
    EmptyRow = 4, //Empty rows, has to destroy another line to clear eat, and not letting any piece filling it. (param1) -> nb Lines
    [Prefixe(null)]
    [Suffixe(" rows")]
    [Description("Vision Block")]
    VisionBlock = 5, //Rows that will go over yours to hide your gameplay (param1) -> nbRows, (param2) -> seconds
    [Prefixe(null)]
    [Suffixe(null)]
    [Description("Forced Piece")]
    ForcedPiece = 6, //Piece that will drop from above. (param1) = Letter or -1 for random or -2 for single block, (param2) = nbRotation or -1 for random
    [Prefixe(null)]
    [Suffixe(null)]
    [Description("Drill")]
    Drill = 7, //Creates a hole in your pieces. (param1) = number of upper blocks before hole
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Air Piece")]
    AirPiece = 8, //Makes your next (param1) pieces transparent
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Forced Block")]
    ForcedBlock = 9, //(param2) blocks that are added to the next (param1) pieces
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Upside Down")]
    MirrorMirror = 10, //Reverse the camera on (param2) (0 = x, 1 = y, 2 = xy) axis for (param1) pieces
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Intoxication")]
    Intoxication = 11, //Makes you drunk for (param1) pieces
    [Prefixe(null)]
    [Suffixe(null)]
    [Description("Drone")]
    Drone = 12, //Invokes a drone that drops (param1) (number) (param2) (type) rows until destroyed
    [Prefixe(null)]
    [Suffixe(" rows")]
    [Description("Shift")]
    Shift = 13, //Shift (param1) rows on the left or right
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Gate")]
    Gate = 14, //A gate of light rows with (param1) cooldown
    [Prefixe(null)]
    [Suffixe(" notes")]
    [Description("Sheet Music")]
    SheetMusic = 15, //A sheet music with (param1) notes, with (param2) air lines thrown if failed
    [Prefixe(null)]
    [Suffixe(" lines")]
    [Description("Shrink")]
    Shrink = 16, //Shrinks playfield by (param1)
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Old School")]
    OldSchool = 17, //reduces DAS, prevent Holding, Hard Dropping, Lock Delay, and apply a custom skin to pieces for (param1) pieces and (param2) gravity
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Screwed")]
    Screwed = 18, //prevents rotations for (param1) pieces
    [Prefixe(null)]
    [Suffixe(" moves")]
    [Description("Drop Bomb")]
    DropBomb = 19, //drop any next current piece in (param1) moves
    [Prefixe(null)]
    [Suffixe(" deep")]
    [Description("Tunnel")]
    Tunnel = 20, //Destroy a random column of (param1) number of blocks from the top
    [Prefixe(null)]
    [Suffixe(" beats")]
    [Description("Rhythm Mania")]
    RhythmMania = 21, //Force you to play in a rhythm for the next (param1) moves, with (param2) emptyRows on miss
    [Prefixe(null)]
    [Suffixe(" lines")]
    [Description("Line Break")]
    LineBreak = 22, //The next (param1) lines aren't destroyed, but set to the bottom of the playfield
    [Prefixe(null)]
    [Suffixe(" attacks")]
    [Description("Shelter")]
    Shelter = 23, //The next (param1) attacks are ignored.
    [Prefixe(null)]
    [Suffixe(" columns")]
    [Description("Ascension")]
    Ascension = 24, //Ascend a (param1) wide column of (param2) lines
}