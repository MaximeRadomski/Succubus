using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Realm
{
    [Description("none")]
    None = -1,
    [Description("each combo adds 10% attack point.")]
    Hell = 0,
    [Description("quadruple lines destroy 1 opponent row.")]
    Earth = 1,
    [Description("twists lower all your cooldowns by 1.")]
    Heaven = 2,
    [Description("the end.")]
    End = 3
}
