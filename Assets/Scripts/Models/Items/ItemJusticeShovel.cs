using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemJusticeShovel : Item
{
    public ItemJusticeShovel()
    {
        Id = 10;
        Name = ItemsData.Items[Id];
        Description = $"clears {Highlight("2")} random adjacent {Highlight("columns")} of the playfield.";
        Rarity = Rarity.Rare;
        Cooldown = 14;
    }

    protected override object Effect()
    {
        if (_character.DiamondBlocks > 0 && Constants.CanceledDiamondBlocks < _character.DiamondBlocks)
        {
            ++Constants.CanceledDiamondBlocks;
            return base.Effect();
        }
        var leftColumnId = Random.Range(0, 8);
        _gameplayControler.DeleteColumn(leftColumnId);
        _gameplayControler.DeleteColumn(leftColumnId + 1);
        return base.Effect();
    }
}
