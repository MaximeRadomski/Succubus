using UnityEngine;

public abstract class Item : Loot
{
    public int Id;
    public string Name;
    public string Description;
    public Rarity Rarity;
    public int Cooldown;

    protected Character _character;
    protected GameplayControler _gameplayControler;

    public virtual void Init(Character character, GameplayControler gameplayControler)
    {
        _character = character;
        _gameplayControler = gameplayControler;
    }

    public virtual bool Activate()
    {
        if (Constants.CurrentItemCooldown - _character.ItemMaxCooldownReducer > 0)
            return false;
        _gameplayControler.Instantiator.PopText(Name.ToLower(), new Vector2(4.5f, 17.4f));
        _gameplayControler.FadeBlocksOnText();
        Constants.ResetCurrentItemCooldown();
        _gameplayControler.UpdateItemAndSpecialVisuals();
        return true;
    }
}
