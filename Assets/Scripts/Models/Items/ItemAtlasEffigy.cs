﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAtlasEffigy : Item
{
    public ItemAtlasEffigy()
    {
        Id = 27;
        Name = ItemsData.Items[Id];
        Description = $"{Highlight("+50% damage")} until the end of the fight, but your playfield is {Highlight("reduced")} in height by {Highlight("5 blocks")}.";
        Rarity = Rarity.Legendary;
        Cooldown = 16;
    }

    protected override void Effect()
    {
        var oldAttack = _character.GetAttackNoBoost();
        _character.BoostAttack += (Mathf.RoundToInt(oldAttack * 0.5f));

        _gameplayControler.ShrinkPlayHeight(5);
        _gameplayControler.CharacterInstanceBhv.Boost(_character.Realm, 2.0f);
        base.Effect();
    }
}
