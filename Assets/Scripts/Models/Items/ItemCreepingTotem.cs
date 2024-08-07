﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCreepingTotem : Item
{
    public ItemCreepingTotem()
    {
        Id = 17;
        Name = ItemsData.Items[Id];
        Description = $"deals {Highlight("4 times")} your damage to your opponent, but create a hole in your playfield.";
        Rarity = Rarity.Common;
        Cooldown = 6;
    }

    protected override void Effect()
    {
        if (_character.DiamondBlocks > 0 && Cache.CanceledDiamondBlocks < _character.DiamondBlocks)
        {
            ++Cache.CanceledDiamondBlocks;
            _gameplayControler.SceneBhv.DamageOpponent(4 * _character.GetAttack(), null, Realm.Earth);
            base.Effect();
        }
        var highestX = -1;
        var highestY = Cache.PlayFieldMinHeight - 1;
        for (int x = 0; x < Constants.PlayFieldWidth; ++x)
        {
            var highestYOnX = _gameplayControler.GetHighestBlockOnX(x);
            if (highestYOnX > highestY)
            {
                highestX = x;
                highestY = highestYOnX;
            }
        }
        var selectedX = Random.Range(0, Constants.PlayFieldWidth);
        var selectedY = _gameplayControler.GetHighestBlockOnX(selectedX);
        if (selectedY < Cache.PlayFieldMinHeight + 3 && highestY >= Cache.PlayFieldMinHeight + 3)
        {
            selectedY = highestY;
            selectedX = highestX;
        }
        if (selectedY >= Cache.PlayFieldMinHeight)
        {
            var centerY = Random.Range(selectedY >= Cache.PlayFieldMinHeight + 3 ? Cache.PlayFieldMinHeight + 1 : Cache.PlayFieldMinHeight, selectedY - 2);
            while (centerY < 0 || _gameplayControler.PlayFieldBhv.Grid[selectedX, centerY]?.GetComponent<BlockBhv>()?.Indestructible == true)
                ++centerY;
            DestroyBlock(selectedX, centerY);
            DestroyBlock(selectedX, centerY + 1);
            DestroyBlock(selectedX, centerY - 1);
            DestroyBlock(selectedX - 1, centerY);
            DestroyBlock(selectedX + 1, centerY);
        }
        _gameplayControler.SceneBhv.DamageOpponent(4 * _character.GetAttack(), null, Realm.Earth);
        base.Effect();
    }

    private void DestroyBlock(int roundedX, int roundedY)
    {
        if (roundedX < 0 || roundedX >= Constants.PlayFieldWidth
            || roundedY < Cache.PlayFieldMinHeight || roundedY >= Constants.PlayFieldHeight
            || _gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY]?.GetComponent<BlockBhv>()?.Indestructible == true)
            return;
        _gameplayControler.Instantiator.NewFadeBlock(Realm.Earth, new Vector3(roundedX, roundedY, 0.0f), 5, 0);
        if (_gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY] != null)
        {
            MonoBehaviour.Destroy(_gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY].gameObject);
            _gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY] = null;
        }
    }
}
