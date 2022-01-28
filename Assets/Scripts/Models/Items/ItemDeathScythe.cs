using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDeathScythe : Item
{
    private int favor = 0;
    private int unfavor = 0;
    private int percentBonus = 50;

    public ItemDeathScythe()
    {
        Id = 25;
        Name = ItemsData.Items[Id];
        Description = $"kiwi";
        Rarity = Rarity.Legendary;
        Cooldown = -1;
        IsUsesBased = true;
        Uses = 1;
    }

    private void GetFavorUnfavor()
    {
        var run = PlayerPrefsHelper.GetRun();
        favor = run.DeathScytheAscension * percentBonus;
        if (favor > 100)
            favor = 100;
        unfavor = 100 - favor;
        if (unfavor < 0)
            unfavor = 0;
    }

    public override string GetDescription()
    {
        GetFavorUnfavor();
        return $"{Highlight($"{favor}%")} chance of killing your opponent, {Highlight($"{unfavor}%")} chance of killing you. switches {percentBonus}% in your favor each time you ascend.\nbreaks on use.";
    }

    protected override object Effect()
    {
        GetFavorUnfavor();
        var result = Random.Range(0, 100);
        if (result < favor)
            ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).KillOpponent();
        else
            _gameplayControler.GameOver();
        PlayerPrefsHelper.ResetCurrentItem();
        _gameplayControler.CharacterItem = PlayerPrefsHelper.GetCurrentItem();
        _gameplayControler.UpdateItemAndSpecialVisuals();
        return base.Effect();
    }
}
