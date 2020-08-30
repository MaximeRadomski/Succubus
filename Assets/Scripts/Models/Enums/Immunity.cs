using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Immunity
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
