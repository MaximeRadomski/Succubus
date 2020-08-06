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

    public void HandleOpacityOnLock(float percent)
    {
        foreach (Transform child in transform)
        {
            var tmpColor = child.gameObject.GetComponent<SpriteRenderer>().color;
            child.gameObject.GetComponent<SpriteRenderer>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, percent + 0.25f);
        }
    }

    public void SetColor(Color color)
    {
        foreach (Transform child in transform)
        {
            var tmpColor = child.gameObject.GetComponent<SpriteRenderer>().color;
            child.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
