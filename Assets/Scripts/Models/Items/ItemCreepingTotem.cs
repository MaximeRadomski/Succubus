using System.Collections;
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

    protected override object Effect()
    {
        if (_character.DiamondBlocks > 0 && Constants.CanceledDiamondBlocks < _character.DiamondBlocks)
        {
            ++Constants.CanceledDiamondBlocks;
            _gameplayControler.SceneBhv.DamageOpponent(4 * _character.GetAttack(), null, Realm.Earth);
            return base.Effect();
        }
        var highestX = -1;
        var highestY = Constants.HeightLimiter - 1;
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
        if (selectedY < Constants.HeightLimiter + 3 && highestY >= Constants.HeightLimiter + 3)
        {
            selectedY = highestY;
            selectedX = highestX;
        }
        if (selectedY >= Constants.HeightLimiter)
        {
            var centerY = Random.Range(selectedY >= Constants.HeightLimiter + 3 ? Constants.HeightLimiter + 1 : Constants.HeightLimiter, selectedY - 2);
            while (centerY < 0 || _gameplayControler.PlayFieldBhv.Grid[selectedX, centerY]?.GetComponent<BlockBhv>()?.Indestructible == true)
                ++centerY;
            DestroyBlock(selectedX, centerY);
            DestroyBlock(selectedX, centerY + 1);
            DestroyBlock(selectedX, centerY - 1);
            DestroyBlock(selectedX - 1, centerY);
            DestroyBlock(selectedX + 1, centerY);
        }
        _gameplayControler.SceneBhv.DamageOpponent(4 * _character.GetAttack(), null, Realm.Earth);
        return base.Effect();
    }

    private void DestroyBlock(int roundedX, int roundedY)
    {
        if (roundedX < 0 || roundedX >= Constants.PlayFieldWidth
            || roundedY < Constants.HeightLimiter || roundedY >= Constants.PlayFieldHeight
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
