using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepInstanceBhv : MonoBehaviour
{
    private SpriteRenderer _step;
    private SpriteRenderer _stepVision;
    private SpriteRenderer _stepLoot;
    private SpriteRenderer _stepOpponent;

    private bool _hasInit;

    public void Init()
    {
        _step = GetComponent<SpriteRenderer>();
        _stepVision = transform.Find("StepVision").GetComponent<SpriteRenderer>();
        _stepLoot = transform.Find("StepLoot").GetComponent<SpriteRenderer>();
        _stepOpponent = transform.Find("StepOpponent").GetComponent<SpriteRenderer>();
        _hasInit = true;
    }

    public void UpdateVisual(Step step)
    {
        if (!_hasInit)
            Init();
        _step.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Steps_" + step.StepType.GetHashCode());
        _stepVision.enabled = step.LandLordVision;
        _stepLoot.enabled = step.LootType != LootType.None;
        _stepLoot.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Steps_" + (2 + step.LootType.GetHashCode()));
        _stepOpponent.enabled = _stepLoot.enabled;
        _stepOpponent.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Steps_" + (6 + step.LootType.GetHashCode()));
    }
}
