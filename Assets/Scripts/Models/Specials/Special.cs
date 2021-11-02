using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Special
{
    public bool IsReactivable = false;
    public bool CanReactivate;

    protected Character _character;
    protected GameplayControler _gameplayControler;

    public virtual void Init(Character character, GameplayControler gameplayControler)
    {
        _character = character;
        _gameplayControler = gameplayControler;
    }

    public virtual bool Activate()
    {
        if (Cache.SelectedCharacterSpecialCooldown > 0)
            return false;
        ResetCooldown();
        _gameplayControler.Instantiator.PopText(_character.SpecialName.ToLower(), new Vector2(4.5f, 17.4f));
        _gameplayControler.FadeBlocksOnText();
        return true;
    }

    public virtual bool Reactivate()
    {
        return true;
    }

    public virtual void ResetCooldown()
    {
        Cache.SelectedCharacterSpecialCooldown = _character.Cooldown - _character.SpecialTotalCooldownReducer;
        if (Cache.SelectedCharacterSpecialCooldown < 1)
            Cache.SelectedCharacterSpecialCooldown = 1;
    }

    public virtual void OnNewPiece(GameObject piece)
    {

    }

    public virtual void OnPieceLocked(GameObject piece)
    {

    }

    public virtual void OnLinesCleared(int nbLines, bool isB2B)
    {
        Cache.SelectedCharacterSpecialCooldown -= nbLines * _character.SpecialCooldownReducer;
    }

    public virtual void OnPerfectClear()
    {
        Cache.SelectedCharacterSpecialCooldown = 0;
    }
}
