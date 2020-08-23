using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public int Id;
    public string Name;
    public string Description;

    protected Character _character;
    protected GameplayControler _gameplayControler;

    public virtual void Init(Character character, GameplayControler gameplayControler)
    {
        _character = character;
        _gameplayControler = gameplayControler;
    }

    public virtual bool Activate()
    {
        if (_gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked)
            return false;
        _gameplayControler.Instantiator.PopText(Name.ToLower(), new Vector2(4.5f, 17.4f));
        _gameplayControler.FadeBlocksOnText();
        PlayerPrefsHelper.ResetCurrentItem();
        return true;
    }
}
