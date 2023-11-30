using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMoonGun : Item
{
    public ItemMoonGun()
    {
        Id = 29;
        Name = ItemsData.Items[Id];
        Description = $"{Highlight("cleans")} your playfield but shrinks it to a height of 10 lines for the next 40 pieces.";
        Rarity = Rarity.Legendary;
        Cooldown = 12;
    }

    protected override void Effect()
    {
        int end = _gameplayControler.GetHighestBlock();
        for (int y = Cache.PlayFieldMinHeight; y <= end; ++y)
        {
            if (y >= 40)
                break;
            _gameplayControler.DeleteLine(y);
        }
        _gameplayControler.CheckForLineBreaks();
        _gameplayControler.ResetPlayHeight(destroyLimiter: false);
        _gameplayControler.ShrinkPlayHeight(10);
        Cache.HeightLimiterResetPieces = 40;
        base.Effect();
    }
}
