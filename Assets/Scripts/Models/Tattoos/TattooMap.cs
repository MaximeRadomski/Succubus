using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooMap : Tattoo
{
    public TattooMap()
    {
        Id = 52;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "all steps";
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.MapAquired = true;
        var run = PlayerPrefsHelper.GetRun();
        var stepsService = new StepsService();
        var originStep = stepsService.GetStepOnPos(50, 50, run.Steps);
        stepsService.GenerateAdjacentSteps(run, character, originStep, run.MaxSteps - run.CurrentStep);
        PlayerPrefsHelper.SaveRun(run);
    }

    protected override void CustomRemove(Character character)
    {
        character.MapAquired = false;
    }

    public override string GetDescription()
    {
        return $"reveals {StatToString()} of the current level.";
    }
}
