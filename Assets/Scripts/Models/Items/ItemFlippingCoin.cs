using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlippingCoin : Item
{
    public ItemFlippingCoin()
    {
        Id = 11;
        Name = ItemsData.Items[Id];
        Description = $"{Highlight("25% chance")} of:\n- canceling opponent next attacks.\n- canceling your special cooldown.\n- canceling the last piece locked.\n- canceling the 2 last rows.";
        Rarity = Rarity.Common;
        Cooldown = 3;
    }

    protected override void Effect()
    {
        var id = Random.Range(0, 4);
        switch (id)
        {
            case 0:
                Cache.IsNextOpponentAttackCanceled = true;
                _gameplayControler.Instantiator.PopText("opponent", _gameplayControler.CharacterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
                break;
            case 1:
                Cache.SelectedCharacterSpecialCooldown = 0;
                this._gameplayControler.UpdateItemAndSpecialVisuals();
                _gameplayControler.Instantiator.PopText("special", _gameplayControler.CharacterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
                break;
            case 2:
                SpecialPenitence();
                _gameplayControler.Instantiator.PopText("piece", _gameplayControler.CharacterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
                break;
            case 3:
                Last2Rows();
                _gameplayControler.Instantiator.PopText("lines", _gameplayControler.CharacterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
                break;
        }

        base.Effect();
    }

    private void SpecialPenitence()
    {
        for (int childId = _gameplayControler.PlayFieldBhv.gameObject.transform.childCount - 1; childId >= 0; --childId)
        {
            var child = _gameplayControler.PlayFieldBhv.gameObject.transform.GetChild(childId);
            var childPiece = child.GetComponent<Piece>();
            if (childPiece == null || childPiece.transform.childCount <= 0 || child.gameObject.name.Contains("Row") || child.gameObject.name == Constants.GoFilledTarget)
                continue;
            else
            {
                foreach (Transform block in child.transform)
                {
                    int roundedX = Mathf.RoundToInt(block.transform.position.x);
                    int roundedY = Mathf.RoundToInt(block.transform.position.y);

                    if (_gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY].gameObject != null)
                        MonoBehaviour.Destroy(_gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY].gameObject);
                    _gameplayControler.PlayFieldBhv.Grid[roundedX, roundedY] = null;
                }
                MonoBehaviour.Destroy(child.gameObject);
                break;
            }
        }
        _gameplayControler.DropGhost();
    }

    private void Last2Rows()
    {
        int nbRows = 2;
        int start = _gameplayControler.GetHighestBlock();
        int end = start - (nbRows - 1);
        for (int y = start; y >= end; --y)
        {
            if (y < Cache.PlayFieldMinHeight)
                break;
            _gameplayControler.DeleteLine(y);
        }
        _gameplayControler.ClearLineSpace();
        _gameplayControler.DropGhost();
        base.Effect();
    }
}
