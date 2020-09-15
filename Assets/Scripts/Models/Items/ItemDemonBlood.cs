using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDemonBlood : Item
{
    public override void Init(Character character, GameplayControler gameplayControler)
    {
        base.Init(character, gameplayControler);
        Id = 1;
        Name = "Demon Blood";
        Description = "Clear all black rows";
        Rarity = Rarity.Common;
        Cooldown = 15;
    }

    public override bool Activate()
    {
        return base.Activate();
    }
}
