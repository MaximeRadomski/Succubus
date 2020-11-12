using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class Item : Loot
{
    public int Id;
    public string Name;
    public string Description;
    public int Cooldown;

    protected Character _character;
    protected GameplayControler _gameplayControler;
    protected GameObject _attackLine;
    
    private Vector3 _popPosition;
    private Func<object> _soundFunc;

    public Item()
    {
        LootType = LootType.Item;
    }

    public virtual bool Activate(Character character, GameplayControler gameplayControler, Func<object> soundFunc)
    {
        _character = character;
        _gameplayControler = gameplayControler;
        _soundFunc = soundFunc;
        if (Constants.CurrentItemCooldown > 0)
            return false;
        _gameplayControler.Instantiator.PopText(Name.ToLower(), new Vector2(4.5f, 17.4f));
        _gameplayControler.FadeBlocksOnText();
        Constants.ResetCurrentItemCooldown(character, this);
        _gameplayControler.UpdateItemAndSpecialVisuals();
        _popPosition = new Vector3(4.5f, 4.5f, 0.0f);
        _attackLine = gameplayControler.Instantiator.NewAttackLine(gameplayControler.CharacterInstanceBhv.transform.position, _popPosition, character.Realm, linear: false,
            Helper.GetSpriteFromSpriteSheet("Sprites/Items_" + Id.ToString("00")), Effect);
        _gameplayControler.SceneBhv.Paused = true;
        return true;
    }

    protected void CancelOrEndItem()
    {
        _gameplayControler.SceneBhv.Paused = false;
    }

    protected virtual object Effect()
    {
        _soundFunc.Invoke();
        CancelOrEndItem();
        return true;
    }
}
