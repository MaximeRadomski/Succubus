using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrenade : Item
{
    public override void Init(Character character, GameplayControler gameplayControler)
    {
        base.Init(character, gameplayControler);
        Id = 2;
        Name = "Grenade";
        Description = "Clear your last four rows";
        Rarity = Rarity.Common;
    }

    public override bool Activate()
    {
        base.Activate();
        _gameplayControler.SceneBhv.Paused = true;
        _gameplayControler.ClearFromTop(4);
        _gameplayControler.SceneBhv.Paused = false;
        PlayerPrefsHelper.ResetCurrentItem();
        _gameplayControler.UpdateItemAndSpecialVisuals();
        return true;
    }
}
