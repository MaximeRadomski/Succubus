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

    public void UpdateVisual(Step step, Run run, bool mapAquired)
    {
        if (!_hasInit)
            Init();
        if (step.Discovered || mapAquired)
        {
            _step.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Steps_" + ((int)step.StepType + ((int)run.CurrentRealm * 16)));
        }
        else
        {
            _step.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Steps_" + ((int)run.CurrentRealm * 16));
        }
        _stepVision.enabled = step.LandLordVision;
        _stepVision.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (Constants.StepsAssetsVisionId + ((int)run.CurrentRealm * Constants.StepsAssetsCount)));
        _stepLoot.enabled = step.LootType != LootType.None;
        _stepLoot.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (Constants.StepsAssetsLootStart + (int)step.LootType + ((int)run.CurrentRealm * Constants.StepsAssetsCount)));
        _stepOpponent.enabled = _stepLoot.enabled;
        var rarity = Rarity.Common;
        if (step.LootType == LootType.Character)
            rarity = Rarity.Legendary;
        else if (step.LootType == LootType.Item)
            rarity = ItemsData.GetItemFromName(ItemsData.Items[step.LootId]).Rarity;
        else if (step.LootType == LootType.Resource)
            rarity = Rarity.Common;
        else if (step.LootType == LootType.Tattoo)
            rarity = TattoosData.GetTattooFromName(TattoosData.Tattoos[step.LootId]).Rarity;
        _stepOpponent.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (Constants.StepsAssetsOpponentStart + (int)rarity + ((int)run.CurrentRealm * Constants.StepsAssetsCount)));
    }
}
