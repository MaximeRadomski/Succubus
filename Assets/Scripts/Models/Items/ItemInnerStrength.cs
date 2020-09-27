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

    public override bool Activate(Character character, GameplayControler gameplayControler)
    {
        if (!base.Activate(character, gameplayControler))
            return false;
        _oldAttack = character.Attack;
        character.Attack = _oldAttack * 2;
        gameplayControler.Invoke(nameof(AfterDelay), 4.0f);
        return true;
    }

    private void AfterDelay()
    {
        _character.Attack = _oldAttack;
    }
}