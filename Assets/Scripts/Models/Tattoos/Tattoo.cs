using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tattoo : Loot
{
    public int Id;
    public string Name;
    public int Stat;
    public string Description;
    public Rarity Rarity;
    public int Level = 1;
    public int MaxLevel;

    protected Character _character;
    protected GameplayControler _gameplayControler;

    public virtual void Init(Character character, GameplayControler gameplayControler)
    {
        _character = character;
        _gameplayControler = gameplayControler;
    }

    public virtual void ApplyToCharacter(Character character)
    {

    }

    protected string StatToString(string before = "", string after = "")
    {
        return Constants.MaterialHell_4_3 + before + (Stat * Level).ToString() + after + Constants.MaterialEnd;
    }
}
