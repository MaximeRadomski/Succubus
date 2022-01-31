using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialWisdom : Special
{
    private int _nbPiece;

    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        _nbPiece = 1;
        OnNewPiece(_gameplayControler.CurrentPiece);
        return true;
    }

    public override void OnNewPiece(GameObject piece)
    {
        base.OnNewPiece(piece);
        if (_nbPiece <= 0)
            return;
        foreach (Transform child in piece.transform)
        {
            _gameplayControler.Instantiator.NewGravitySquare(child.gameObject, Realm.Hell.ToString());
        }
        _gameplayControler.DropGhost();
    }

    public override void OnLinesCleared(int nbLines, bool isB2B)
    {
        base.OnLinesCleared(nbLines, isB2B);
    }

    public override void OnPieceLocked(GameObject piece)
    {
        base.OnPieceLocked(piece);
        --_nbPiece;
        if (_nbPiece < 0)
            return;
        foreach (Transform block in piece.transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);

            TryDestroy(x, y + 1, block.parent);
            TryDestroy(x + 1, y + 1, block.parent);
            TryDestroy(x + 1, y, block.parent);
            TryDestroy(x + 1, y - 1, block.parent);
            TryDestroy(x, y - 1, block.parent);
            TryDestroy(x - 1, y - 1, block.parent);
            TryDestroy(x - 1, y, block.parent);
            TryDestroy(x - 1, y + 1, block.parent);
        }
        for (int i = piece.transform.childCount - 1; i >= 0; --i)
        {
            var block = piece.transform.GetChild(i);
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            MonoBehaviour.Destroy(_gameplayControler.PlayFieldBhv.Grid[x, y].gameObject);
            _gameplayControler.PlayFieldBhv.Grid[x, y] = null;
        }
    }

    private void TryDestroy(int x, int y, Transform parent)
    {
        if (x < 0 || x >= Constants.PlayFieldWidth
            || y < Cache.PlayFieldMinHeight || y >= Constants.PlayFieldHeight
            || _gameplayControler.PlayFieldBhv.Grid[x, y]?.parent == parent
            || _gameplayControler.PlayFieldBhv.Grid[x, y]?.GetComponent<BlockBhv>()?.Indestructible == true)
            return;
        _gameplayControler.Instantiator.NewFadeBlock(Realm.Hell, new Vector3(x, y, 0.0f), 5, 0);
        if (_gameplayControler.PlayFieldBhv.Grid[x, y] != null)
        {
            MonoBehaviour.Destroy(_gameplayControler.PlayFieldBhv.Grid[x, y].gameObject);
            _gameplayControler.PlayFieldBhv.Grid[x, y] = null;
        }
    }
}
