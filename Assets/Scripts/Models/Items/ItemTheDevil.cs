using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTheDevil : Item
{
    public ItemTheDevil()
    {
        Id = 18;
        Name = ItemsData.Items[Id];
        Description = $"replaces {Highlight("all the pieces")} in your piece preview by the current one.";
        Rarity = Rarity.Rare;
        Cooldown = 8;
    }

    protected override void Effect()
    {
        for (int i = 0; i < 5; ++i)
            _gameplayControler.Bag = _gameplayControler.Bag.ReplaceChar(i, _gameplayControler.CurrentPiece.GetComponent<Piece>().Letter[0]);
        _gameplayControler.UpdateNextPieces();
        base.Effect();
    }
}
