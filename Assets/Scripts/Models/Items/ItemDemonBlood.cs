using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDemonBlood : Item
{
    public ItemDemonBlood()
    {
        Id = 1;
        Name = ItemsData.Items[Id];
        Description = $"clears {Highlight("6 dark rows")}.";
        Rarity = Rarity.Common;
        Cooldown = 6;
    }

    protected override object Effect()
    {
        _gameplayControler.CheckForDarkRows(4);
        _gameplayControler.ClearLineSpace();
        return base.Effect();
    }
}
