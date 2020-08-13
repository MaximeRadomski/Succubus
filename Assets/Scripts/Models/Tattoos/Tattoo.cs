using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tattoo
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
}
