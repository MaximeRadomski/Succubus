using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAttack
{
    public AttackType AttackType;
    public int Param1;
    public int Param2; //For Grey lines -> number of holes in each line (min 1). For Black lines -> Nothing. For White lines -> lines cooldown.

    public OpponentAttack(AttackType type, int param1, int param2 = 0)
    {
        AttackType = type;
        Param1 = param1;
        Param2 = param2;
    }
}
