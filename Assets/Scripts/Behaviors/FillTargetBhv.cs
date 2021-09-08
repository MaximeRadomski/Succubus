using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillTargetBhv : FrameRateBehavior
{
    private Realm _characterRealm;
    private GameplayControler _gameplayControler;
    private Vector3 _resetPosition;
    private float _delay = 2.0f;
    private GameObject _filledTarget;

    private bool _isMovingToHolePosition;
    private bool _isMovingBackToResetPosition;

    public void Init(Realm characterRealm, GameplayControler gameplayControler)
    {
        _characterRealm = characterRealm;
        _gameplayControler = gameplayControler;
        _resetPosition = transform.position;

        Invoke(nameof(CheckForSingleHole), _delay);
    }

    private void CheckForSingleHole()
    {
        if (TryFindSingleHole(out var holePosition))
        {
            _filledTarget = _gameplayControler.Instantiator.NewPiece("Filled", "Target", new Vector3(holePosition[0], holePosition[1], 0.0f));
            _filledTarget.name = Constants.GoFilledTarget;
            _gameplayControler.AddToPlayField(_filledTarget);
            _isMovingToHolePosition = true;
        }
        else
            Invoke(nameof(CheckForSingleHole), _delay);
    }

    protected override void FrameUpdate()
    {
        if (_filledTarget != null && _filledTarget.name != Constants.GoFilledTarget)
        {
            _filledTarget = GameObject.Find(Constants.GoFilledTarget);
        }
        if (_isMovingToHolePosition && (_filledTarget == null || _filledTarget.transform.childCount == 0))
        {
            _isMovingToHolePosition = false;
            _isMovingBackToResetPosition = true;
        }
        if (_isMovingToHolePosition)
        {
            transform.position = Vector3.Lerp(transform.position, _filledTarget.transform.GetChild(0).position, 0.05f);
            if (Helper.VectorEqualsPrecision(transform.position, _filledTarget.transform.GetChild(0).position, 0.2f))
            {
                _isMovingToHolePosition = false;
                transform.position = _filledTarget.transform.GetChild(0).position;
                int x = Mathf.RoundToInt(_filledTarget.transform.GetChild(0).position.x);
                int y = Mathf.RoundToInt(_filledTarget.transform.GetChild(0).position.y);
                var tmpPiece = _gameplayControler.Instantiator.NewPiece("D", _characterRealm.ToString(), new Vector3(x, y, 0.0f));
                _gameplayControler.AddToPlayField(tmpPiece);
                StartCoroutine(Helper.ExecuteAfterDelay(0.25f, () => { _isMovingBackToResetPosition = true; return true; }, lockInputWhile: false));
            }
        }
        else if (_isMovingBackToResetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, _resetPosition, 0.05f);
            if (Helper.VectorEqualsPrecision(transform.position, _resetPosition, 0.2f))
            {
                _isMovingBackToResetPosition = false;
                transform.position = _resetPosition;
                Invoke(nameof(CheckForSingleHole), _delay);
            }
        }
    }

    private bool TryFindSingleHole(out int[] position)
    {
        List<int[]> availableSingleHoles = new List<int[]>();
        for (int y = Constants.PlayFieldHeight - 1; y >= Constants.HeightLimiter; --y)
        {
            for (int x = 0; x < Constants.PlayFieldWidth; ++x)
            {
                if (_gameplayControler.PlayFieldBhv.Grid[x, y] == null
                    && (y + 1 >= Constants.PlayFieldHeight || _gameplayControler.PlayFieldBhv.Grid[x, y + 1] != null)
                    && (y - 1 < Constants.HeightLimiter || _gameplayControler.PlayFieldBhv.Grid[x, y - 1] != null)
                    && (x - 1 < Constants.HeightLimiter || _gameplayControler.PlayFieldBhv.Grid[x - 1, y] != null)
                    && (x + 1 >= Constants.PlayFieldWidth || _gameplayControler.PlayFieldBhv.Grid[x + 1, y] != null))
                {
                    availableSingleHoles.Add(new int[] { x, y });
                    //var tmpPiece = _gameplayControler.Instantiator.NewPiece("D", _characterRealm.ToString(), new Vector3(x, y, 0.0f));
                    //_gameplayControler.AddToPlayField(tmpPiece);
                }
            }
        }
        position = availableSingleHoles != null && availableSingleHoles.Count > 0 ? availableSingleHoles[Random.Range(0, availableSingleHoles.Count - 1)] : null;
        return position != null;
        //for (int y = Constants.PlayFieldHeight - 1; y >= 0; --y)
        //{
        //    if (_gameplayControler.HasLine(y))
        //    {
        //        _gameplayControler.DeleteLine(y);
        //    }
        //}
        //_gameplayControler.ClearLineSpace();
    }
}
