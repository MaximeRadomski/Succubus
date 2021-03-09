using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBoxBhv : FrameRateBehavior
{
    public void Init(string subjectName, string secondaryName)
    {
        _opponentInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{_run?.CurrentRealm ?? Realm.Hell}Opponents_{CurrentOpponent.Id}");
    }
}
