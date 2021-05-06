using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCreepingTotem : Item
{
    public ItemCreepingTotem()
    {
        Id = 17;
        Name = ItemsData.Items[Id];
        Description = $"deals {Highlight("4 times")} your damages to your opponent, but create a hole in your playfield.";
        Rarity = Rarity.Common;
        Cooldown = 2;
    }

    protected override object Effect()
    {
        var highestX = -1;
        var highestY = -1;
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
        if (selectedY < 3 && highestY >= 3)
        {
            selectedY = highestY;
            selectedX = highestX;
        }
        if (selectedY >= 0)
        {
            var centerY = Random.Range(selectedY >= 3 ? 1 : 0, selectedY - 2);
            while (_gameplayControler.PlayFieldBhv.Grid[selectedX, centerY]?.GetChild(0)?.GetComponent<BlockBhv>()?.Indestructible == true)
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
            || roundedY < 0 || roundedY >= Constants.PlayFieldHeight
            || _gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY]?.GetChild(0)?.GetComponent<BlockBhv>()?.Indestructible == true)
            return;
        _gameplayControler.Instantiator.NewFadeBlock(Realm.Earth, new Vector3(roundedX, roundedY, 0.0f), 5, 0);
        if (_gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY] != null)
        {
            MonoBehaviour.Destroy(_gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY].gameObject);
            _gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY] = null;
        }
    }
}
