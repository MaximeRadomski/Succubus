using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCRTMonitor : Item
{
    public ItemCRTMonitor()
    {
        Id = 16;
        Name = ItemsData.Items[Id];
        Description = $"removes {Highlight("1 out of 2 lines")} on your playfield.";
        Rarity = Rarity.Common;
        Cooldown = 16;
    }

    protected override void Effect()
    {
        _gameplayControler.GameplayOnHold = true;
        for (int i = 0; i < Constants.PlayFieldHeight; i += 2)
            _gameplayControler.DeleteLine(i);
        _gameplayControler.StartCoroutine(Helper.ExecuteAfterDelay(0.2f, () =>
        {
            _gameplayControler.ClearLineSpace();
            _gameplayControler.GameplayOnHold = false;
        }));
        base.Effect();
    }
}
