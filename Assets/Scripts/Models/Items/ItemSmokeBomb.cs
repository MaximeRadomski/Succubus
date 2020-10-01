using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class ItemSmokeBomb : Item
{
    public ItemSmokeBomb()
    {
        Id = 4;
        Name = ItemsData.Items[Id];
        Description = "makes you exit the confrontation";
        Rarity = Rarity.Rare;
        Cooldown = 24;
    }

    public override bool Activate(Character character, GameplayControler gameplayControler)
    {
        if (!base.Activate(character, gameplayControler))
            return false;
        var stepsService = new StepsService();
        var run = PlayerPrefsHelper.GetRun();
        _gameplayControler.SceneBhv.Paused = true;
        var currentStep = stepsService.GetStepOnPos(run.X, run.Y, run.Steps);
        var tmpStep = stepsService.GetClosestAvailableStepFromPos(run.X, run.Y, run);
        stepsService.GenerateAdjacentSteps(run, character, currentStep);
        run.X = tmpStep.X;
        run.Y = tmpStep.Y;
        PlayerPrefsHelper.SaveRun(run);
        gameplayControler.CleanPlayerPrefs();
        NavigationService.LoadBackUntil(Constants.StepsScene);
        return true;
    }
}