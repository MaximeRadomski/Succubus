using UnityEngine;
using System.Collections;

public class ItemInnerStrength : Item
{
    public ItemInnerStrength()
    {
        Id = 5;
        Name = ItemsData.Items[Id];
        Description = "doubles your base attack damages for 4 seconds";
        Rarity = Rarity.Legendary;
        Cooldown = 10;
    }

    private int _oldAttack;

    protected override object Effect()
    {
        _oldAttack = _character.Attack;
        _character.Attack = _oldAttack * 2;
        _gameplayControler.Invoke(nameof(AfterDelay), 4.0f);
        return base.Effect();
    }

    private void AfterDelay()
    {
        _character.Attack = _oldAttack;
    }
}