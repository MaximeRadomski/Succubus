using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class ItemSmokeBomb : Item
{
    public ItemSmokeBomb()
    {
        Id = 4;
        Name = ItemsData.Items[Id];
        Description = $"makes you {Highlight("exit")} the confrontation.";
        Rarity = Rarity.Rare;
        Cooldown = 18;
    }

    protected override object Effect()
    {
        var stepsService = new StepsService();
        var run = PlayerPrefsHelper.GetRun();
        var currentStep = stepsService.GetStepOnPos(run.X, run.Y, run.Steps);
        var tmpStep = stepsService.GetClosestAvailableStepFromPos(run.X, run.Y, run);
        stepsService.GenerateAdjacentSteps(run, _character, currentStep);
        run.X = tmpStep.X;
        run.Y = tmpStep.Y;
        PlayerPrefsHelper.SaveRun(run);
        _gameplayControler.CleanPlayerPrefs();
        NavigationService.LoadBackUntil(Constants.StepsScene);
        return base.Effect();
    }
}