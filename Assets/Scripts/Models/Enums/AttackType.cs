using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum AttackType
{
    None = 0,
    [Description("Dark Rows")]
    DarkRows = 1, //Full rows, can only be destroyed by doing lines (2 clear 1, 3 clear 2, 4 clear 3)
    [Description("Garbage Rows")]
    GarbageRows = 2, //Standard rows with holes in it
    [Description("Light Rows")]
    LightRows = 3, //Full rows with a cooldown, each time a piece locks, it decreases the cooldown. Is destroyed after cooldown.
    [Description("Empty Rows")]
    EmptyRows = 4, //Empty rows, has to destroy another line to clear eat, and not letting any piece filling it
    [Description("Vision Rows")]
    VisionBlock = 5, //Rows that will go over yours to hide your gameplay
}
