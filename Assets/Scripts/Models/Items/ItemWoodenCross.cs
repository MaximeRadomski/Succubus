using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWoodenCross : Item
{
    public ItemWoodenCross()
    {
        Id = 8;
        Name = ItemsData.Items[Id];
        Description = "reset the speed of the room";
        Rarity = Rarity.Rare;
        Cooldown = 10;
    }

    public override bool Activate(Character character, GameplayControler gameplayControler)
    {
        if (!base.Activate(character, gameplayControler))
            return false;
        _gameplayControler.SceneBhv.Paused = true;
        _gameplayControler.SetGravity(1);
        _gameplayControler.SceneBhv.Paused = false;
        return true;
    }
}
