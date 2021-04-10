using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSwap : Special
{
    private GameObject _selector;
    private int _selectedId;
    private float _customUpdateDelay = 0.25f;
    private bool _canReactivate;

    public override bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        if (!base.Activate())
            return false;
        _gameplayControler.SceneBhv.Paused = true;
        Constants.EscapeLocked = true;
        _selector = GameObject.Instantiate(new GameObject());
        var selectorSpriteRenderer = _selector.AddComponent<SpriteRenderer>();
        selectorSpriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Drone_3");
        selectorSpriteRenderer.sortingLayerName = "Effects";
        _selector.transform.position = new Vector3(_gameplayControler.NextPieces[0].transform.position.x - 2.7143f, _gameplayControler.NextPieces[0].transform.position.y + 0.5f, 0.0f);
        _selectedId = -1;
        _canReactivate = true;
        _gameplayControler.StartCoroutine(CustomUpdate());
        return true;
    }

    private IEnumerator CustomUpdate()
    {
        if (_selector != null)
        {
            if (++_selectedId > 4)
                _selectedId = 0;
            _selector.transform.position = new Vector3(_selector.transform.position.x, _gameplayControler.NextPieces[_selectedId].transform.position.y + 0.5f, 0.0f);
            yield return new WaitForSeconds(_customUpdateDelay);
            _gameplayControler.StartCoroutine(CustomUpdate());
        }
    }

    public override void Reactivate()
    {
        if (!_canReactivate)
            return;
        _canReactivate = false;
        Object.Destroy(_selector);
        _gameplayControler.CurrentPiece.GetComponent<Piece>().SetColor(Constants.ColorPlainTransparent);
        _gameplayControler.NextPieces[_selectedId].transform.GetChild(0).GetComponent<Piece>().SetColor(Constants.ColorPlainTransparent);

        _gameplayControler.Instantiator.NewAttackLine(_gameplayControler.CurrentPiece.transform.position, _gameplayControler.NextPieces[_selectedId].transform.position, Realm.Hell, linear: false,
            Helper.GetSpriteFromSpriteSheet("Sprites/Drone_0"), AfterEffect);
        _gameplayControler.Instantiator.NewAttackLine(_gameplayControler.NextPieces[_selectedId].transform.position, _gameplayControler.CurrentPiece.transform.position, Realm.Hell, linear: false,
            Helper.GetSpriteFromSpriteSheet("Sprites/Drone_0"));
    }

    private object AfterEffect()
    {
        _gameplayControler.Bag = _gameplayControler.Bag.Insert(0, _gameplayControler.Bag[_selectedId].ToString());
        _gameplayControler.Bag = _gameplayControler.Bag.ReplaceChar(_selectedId + 1, _gameplayControler.CurrentPiece.GetComponent<Piece>().Letter[0]);
        Object.Destroy(_gameplayControler.CurrentPiece ?? null);
        Object.Destroy(_gameplayControler.CurrentGhost ?? null);
        _gameplayControler.SceneBhv.Paused = false;
        Constants.EscapeLocked = false;
        _gameplayControler.Spawn(false);
        return true;
    }
}
