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

    protected override object Effect()
    {
        _gameplayControler.SceneBhv.DamageOpponent(666, _gameplayControler.CharacterInstanceBhv.gameObject);
        return base.Effect();
    }
}