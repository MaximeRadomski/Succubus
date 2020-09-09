using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum AttackType
{
    None = 0,
    [Description("Dark Row")]
    DarkRow = 1, //Full rows, can only be destroyed by doing lines (2 clear 1, 3 clear 2, 4 clear 3)
    [Description("Garbage Row")]
    GarbageRow = 2, //Standard rows with holes in it
    [Description("Light Row")]
    LightRow = 3, //Full rows with a cooldown, each time a piece locks, it decreases the cooldown. Is destroyed after cooldown.
    [Description("Empty Row")]
    EmptyRow = 4, //Empty rows, has to destroy another line to clear eat, and not letting any piece filling it
    [Description("Vision Block")]
    VisionBlock = 5, //Rows that will go over yours to hide your gameplay
    [Description("Forced Piece")]
    ForcedPiece = 6, //Rows that will go over yours to hide your gameplay
}
