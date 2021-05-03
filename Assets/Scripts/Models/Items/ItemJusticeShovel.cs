using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemJusticeShovel : Item
{
    public ItemJusticeShovel()
    {
        Id = 10;
        Name = ItemsData.Items[Id];
        Description = "clears 2 random adjacent columns of the playfield.";
        Rarity = Rarity.Common;
        Cooldown = 5;
    }

    protected override object Effect()
    {
        var leftColumnId = Random.Range(0, 8);
        _gameplayControler.DeleteColumn(leftColumnId);
        _gameplayControler.DeleteColumn(leftColumnId + 1);
        return base.Effect();
    }
}
