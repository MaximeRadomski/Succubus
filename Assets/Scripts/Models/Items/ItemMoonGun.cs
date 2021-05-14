using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMoonGun : Item
{
    public ItemMoonGun()
    {
        Id = 29;
        Name = ItemsData.Items[Id];
        Description = $"{Highlight("cleans")} your playfield but shrinks it to a height of 6 lines for the next 20 pieces. Your opponent {Highlight("cannot attack")} during this phase.";
        Rarity = Rarity.Legendary;
        Cooldown = 12;
    }

    protected override object Effect()
    {
        int end = _gameplayControler.GetHighestBlock();
        for (int y = Constants.HeightLimiter; y <= end; ++y)
        {
            if (y >= 40)
                break;
            _gameplayControler.DeleteLine(y);
        }
        _gameplayControler.ResetPlayHeight();
        _gameplayControler.ReducePlayHeight(14);
        Constants.HeightLimiterResetLines = 20;
        return base.Effect();
    }
}
