using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Realm
{
    [Description("each combo adds 10% attack point")]
    Hell = 0,
    [Description("quadruple lines destroy 1 opponent row")]
    Earth = 1,
    [Description("consecutive multiple lines lower all cooldowns by 1")]
    Heaven = 2
}
