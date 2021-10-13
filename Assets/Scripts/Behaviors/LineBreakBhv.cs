using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

public class LineBreakBhv : FrameRateBehavior
{
    public List<Sprite> _sprites;

    private List<SpriteRenderer> _renderers;

    public void Init(Realm realm)
    {
        _renderers = new List<SpriteRenderer>();

        for (int i = 0; i < 3; ++i)
        {
            _renderers.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
            _renderers[i].sprite = _sprites[3 * realm.GetHashCode() + i];
        }
    }
}