using UnityEngine;

public abstract class Item : Loot
{
    public int Id;
    public string Name;
    public string Description;
    public int Cooldown;

    protected Character _character;
    protected GameplayControler _gameplayControler;

    public Item()
    {
        LootType = LootType.Item;
    }

    public virtual bool Activate(Character character, GameplayControler gameplayControler)
    {
        _character = character;
        _gameplayControler = gameplayControler;
        if (Constants.CurrentItemCooldown > 0)
            return false;
        _gameplayControler.Instantiator.PopText(Name.ToLower(), new Vector2(4.5f, 17.4f));
        _gameplayControler.FadeBlocksOnText();
        Constants.ResetCurrentItemCooldown(character, this);
        _gameplayControler.UpdateItemAndSpecialVisuals();
        return true;
    }
}
