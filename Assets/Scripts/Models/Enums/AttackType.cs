using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum AttackType
{
    None = 0,
    [Description("Dark Lines")]
    DarkLines = 1, //Full line, can only be destroyed by doing lines (2 clear 1, 3 clear 2, 4 clear 3)
    [Description("Garbage Lines")]
    GarbageLines = 2, //Standard Line with holes in it
    [Description("Light Lines")]
    LightLines = 3 //Full line with a cooldown, each time a piece locks, it decreases the cooldown. Is destroyed after cooldown.
}
