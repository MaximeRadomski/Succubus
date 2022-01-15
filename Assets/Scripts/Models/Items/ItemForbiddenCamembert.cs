using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemForbiddenCamembert : Item
{
    public ItemForbiddenCamembert()
    {
        Id = 9;
        Name = ItemsData.Items[Id];
        Description = $"fills every {Highlight("single holes")}.";
        Rarity = Rarity.Rare;
        Cooldown = 8;
    }

    protected override object Effect()
    {
        for (int y = Constants.PlayFieldHeight - 1; y >= Cache.HeightLimiter; --y)
        {
            for (int x = 0; x < Constants.PlayFieldWidth; ++x)
            {
                if (_gameplayControler.PlayFieldBhv.Grid[x, y] == null
                    && (y + 1 >= Constants.PlayFieldHeight || _gameplayControler.PlayFieldBhv.Grid[x, y + 1] != null)
                    && (y - 1 < Cache.HeightLimiter || _gameplayControler.PlayFieldBhv.Grid[x, y - 1] != null)
                    && (x - 1 < Cache.HeightLimiter || _gameplayControler.PlayFieldBhv.Grid[x - 1, y] != null)
                    && (x + 1 >= Constants.PlayFieldWidth || _gameplayControler.PlayFieldBhv.Grid[x + 1, y] != null))
                {
                    var tmpPiece = _gameplayControler.Instantiator.NewPiece("D", _character.Realm.ToString(), new Vector3(x, y, 0.0f));
                    _gameplayControler.AddToPlayField(tmpPiece);
                }
            }
        }
        for (int y = Constants.PlayFieldHeight - 1; y >= 0; --y)
        {
            if (_gameplayControler.HasLine(y))
            {
                _gameplayControler.DeleteLine(y);
            }
        }
        _gameplayControler.ClearLineSpace();
        return base.Effect();
    }
}
