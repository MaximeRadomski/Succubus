using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPenitence : Special
{
    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked
            || _gameplayControler.PlayFieldBhv == null
            || _gameplayControler.PlayFieldBhv.gameObject.transform.childCount <= 0)
            return false;
        if (!base.Activate())
            return false;
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
        return true;
    }
}
