using UnityEngine;
using System.Collections;

public class ItemHolyGrenade : Item
{
    public ItemHolyGrenade()
    {
        Id = 6;
        Name = ItemsData.Items[Id];
        Description = "clears the whole playfield";
        Rarity = Rarity.Legendary;
        Cooldown = 40;
    }

    public override bool Activate(Character character, GameplayControler gameplayControler)
    {
        if (!base.Activate(character, gameplayControler))
            return false;
        _gameplayControler.SceneBhv.Paused = true;
        int end = _gameplayControler.GetHighestBlock();
        for (int y = 0; y <= end; ++y)
        {
            if (y >= 40)
                break;
            _gameplayControler.DeleteLine(y);
        }
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
        _gameplayControler.SceneBhv.Paused = false;
        return true;
    }
}