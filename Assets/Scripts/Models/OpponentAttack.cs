using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAttack
{
    public AttackType AttackType;
    public int NbAttackRows;
    public int AttackParam; //For Grey lines -> number of holes in each line (min 1). For Black lines -> Nothing. For White lines -> lines cooldown.

    public OpponentAttack(AttackType type, int rows, int param = 0)
    {
        AttackType = type;
        NbAttackRows = rows;
        AttackParam = param;
    }
}
