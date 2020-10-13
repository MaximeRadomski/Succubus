using UnityEngine;
using System.Collections;

public class ItemReverseCrucifix : Item
{
    public ItemReverseCrucifix()
    {
        Id = 7;
        Name = ItemsData.Items[Id];
        Description = "inflicts 666 damages to your opponent (except bosses)";
        Rarity = Rarity.Legendary;
        Cooldown = 40;
    }

    public override bool Activate(Character character, GameplayControler gameplayControler)
    {
        if (!base.Activate(character, gameplayControler))
            return false;
        _gameplayControler.SceneBhv.Paused = true;
        _gameplayControler.SceneBhv.DamageOpponent(666, gameplayControler.CharacterInstanceBhv.gameObject);
        _gameplayControler.SceneBhv.Paused = false;
        return true;
    }

    public void ClearFromTop(int nbRows)
    {
        int start = _gameplayControler.GetHighestBlock();
        int end = start - (nbRows - 1);
        for (int y = start; y >= end; --y)
        {
            if (y < 0)
                break;
            _gameplayControler.DeleteLine(y);
        }
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
    }
}