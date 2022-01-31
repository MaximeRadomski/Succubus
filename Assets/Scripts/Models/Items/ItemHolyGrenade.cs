using UnityEngine;
using System.Collections;

public class ItemHolyGrenade : Item
{
    public ItemHolyGrenade()
    {
        Id = 6;
        Name = ItemsData.Items[Id];
        Description = $"clears the {Highlight("whole playfield")}.";
        Rarity = Rarity.Legendary;
        Cooldown = 40;
    }

    protected override object Effect()
    {
        int end = _gameplayControler.GetHighestBlock();
        for (int y = Cache.PlayFieldMinHeight; y <= end; ++y)
        {
            if (y >= 40)
                break;
            _gameplayControler.DeleteLine(y); 
        }
        _gameplayControler.CheckForLineBreaks();
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
        return base.Effect();
    }
}