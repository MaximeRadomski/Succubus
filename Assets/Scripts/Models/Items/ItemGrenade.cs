using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrenade : Item
{
    public ItemGrenade()
    {
        Id = 2;
        Name = "Grenade";
        Description = "Clear your last four rows";
        Rarity = Rarity.Common;
        Cooldown = 15;
    }

    public override bool Activate()
    {
        if (!base.Activate())
            return false;
        _gameplayControler.SceneBhv.Paused = true;
        _gameplayControler.ClearFromTop(4);
        _gameplayControler.SceneBhv.Paused = false;
        return true;
    }
}
