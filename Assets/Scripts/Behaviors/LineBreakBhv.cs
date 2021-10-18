using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

public class LineBreakBhv : FrameRateBehavior
{
    public List<Sprite> _sprites;

    public void Init(int x)
    {
        if (x == 0)
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _sprites[0];
        else if (x == Constants.PlayFieldWidth - 1)
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _sprites[2];
    }
}