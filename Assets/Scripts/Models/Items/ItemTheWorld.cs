using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTheWorld : Item
{
    public ItemTheWorld()
    {
        Id = 19;
        Name = ItemsData.Items[Id];
        Description = $"stops the time for {Highlight("9 seconds")} (opponent cooldown + gravity).";
        Rarity = Rarity.Rare;
        Cooldown = 6;
    }

    protected override object Effect()
    {
        _gameplayControler.Instantiator.PopText("toki yo tomare!", _gameplayControler.CharacterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
        ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).StopTime(9);
        return base.Effect();
    }
}
