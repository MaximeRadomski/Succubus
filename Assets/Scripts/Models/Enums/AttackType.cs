using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnityEngine;

public enum AttackType
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
    [Suffixe(" lines")]
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
    AirPiece = 8, //Makes your next 'param1' pieces transparent
    [Prefixe(null)]
    [Suffixe(null)]
    [Description("Forced Block")]
    ForcedBlock = 9, //'param2' blocks that are added to the next 'param1' pieces
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Upside Down")]
    MirrorMirror = 10, //Reverse the camera on 'param2' (0 = x, 1 = y, 2 = xy) axis for 'param1' pieces
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Intoxication")]
    Intoxication = 11, //Makes you drunk for 'param1' pieces
    [Prefixe(null)]
    [Suffixe(null)]
    [Description("Drone")]
    Drone = 12, //Invokes a drone that drops 'param1' (number) 'param2' (type) rows until destroyed
    [Prefixe(null)]
    [Suffixe(null)]
    [Description("Shift")]
    Shift = 13, //Shift (param1) rows on the left or right
    [Prefixe(null)]
    [Suffixe(" pieces")]
    [Description("Gate")]
    Gate = 14, //A gate of light rows with (param1) cooldown
    [Prefixe(null)]
    [Suffixe(" notes")]
    [Description("Partition")]
    Partition = 15, //A music partition with (param1) notes, with (param2) air lines thrown if failed
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
    RhythmMania = 21, //Force you to play in rythm for the next (param1) moves
    [Prefixe(null)]
    [Suffixe(" lines")]
    [Description("Line Break")]
    LineBreak = 22, //The next (param1) lines aren't destroyed, but set to the bottom of the playfield
}