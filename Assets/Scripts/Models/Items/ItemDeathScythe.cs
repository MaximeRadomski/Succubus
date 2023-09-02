using UnityEngine;

public class ItemDeathScythe : Item
{
    private int favor = 0;
    private int unfavor = 0;
    private int percentBonus = 7;

    public ItemDeathScythe()
    {
        Id = 25;
        Name = ItemsData.Items[Id];
        Description = $"kiwi";
        Rarity = Rarity.Legendary;
        Cooldown = -1;
        Type = ItemType.KillBased;
    }

    private void GetFavorUnfavor()
    {
        var run = PlayerPrefsHelper.GetRun();
        favor = run.DeathScytheCount * percentBonus;
        if (favor > 100)
            favor = 100;
        if (favor < 0)
            favor = 0;
        unfavor = 100 - favor;
        if (unfavor < 0)
            unfavor = 0;
    }

    public override string GetDescription()
    {
        GetFavorUnfavor();
        return $"{Highlight($"{favor}%")} chance of killing your opponent, {Highlight($"{unfavor}%")} chance of killing you. switches {percentBonus}% in your favor each time you kill and opponent.\nresets on a successful opponent kill.";
    }

    protected override void Effect()
    {
        GetFavorUnfavor();
        var result = Random.Range(0, 100);
        if (result < favor)
        {
            var run = PlayerPrefsHelper.GetRun();
            run.DeathScytheCount = -1; //because will be updated in the kill Opponent
            PlayerPrefsHelper.SaveRun(run);
            _gameplayControler.UpdateItemAndSpecialVisuals();
            ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).KillOpponent();

        }
        else
            _gameplayControler.GameOver();
        base.Effect();
    }

    public override string GetKillBasedText()
    {
        GetFavorUnfavor();
        return $"{favor}%";
    }
}
