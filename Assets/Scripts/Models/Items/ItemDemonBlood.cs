﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDemonBlood : Item
{
    public ItemDemonBlood()
    {
        Id = 1;
        Name = ItemsData.Items[Id];
        Description = "clears 4 dark rows";
        Rarity = Rarity.Common;
        Cooldown = 12;
    }

    public override bool Activate(Character character, GameplayControler gameplayControler)
    {
        if (!base.Activate(character, gameplayControler))
            return false;
        _gameplayControler.SceneBhv.Paused = true;
        _gameplayControler.CheckForDarkRows(4);
        _gameplayControler.ClearLineSpace();
        _gameplayControler.SceneBhv.Paused = false;
        return true;
    }
}
