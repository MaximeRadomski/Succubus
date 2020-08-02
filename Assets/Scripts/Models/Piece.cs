using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public string Letter;
    public float XFromSpawn;
    public float YFromSpawn;
    public bool IsLocked;
    public RotationState RotationState = RotationState.O;
}
