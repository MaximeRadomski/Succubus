using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolyWater : Item
{
    public ItemHolyWater()
    {
        Id = 0;
        Name = ItemsData.Items[Id];
        Description = "clears 4 waste rows";
        Rarity = Rarity.Common;
        Cooldown = 12;
    }

    public override bool Activate(Character character, GameplayControler gameplayControler)
    {
        if (!base.Activate(character, gameplayControler))
            return false;
        _gameplayControler.SceneBhv.Paused = true;
        _gameplayControler.CheckForWasteRows(4);
        _gameplayControler.ClearLineSpace();
        _gameplayControler.SceneBhv.Paused = false;
        return true;
    }
}
