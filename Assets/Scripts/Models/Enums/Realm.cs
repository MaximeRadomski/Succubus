using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Realm
{
    [Description("each combo adds an attack point")]
    Hell = 0,
    [Description("quadruple lines remove one grey row")]
    Earth = 1,
    [Description("consecutive multiple lines lower your cooldown")]
    Heaven = 2
}
