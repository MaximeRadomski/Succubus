using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsSceneBhv : SceneBhv
{
    private Run _run;
    private Character _character;
    private StepService _stepsService;
    private GameObject _stepsContainer;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _run = PlayerPrefsHelper.GetRun();
        _character = PlayerPrefsHelper.GetRunCharacter();
        _stepsService = new StepService();
        if (string.IsNullOrEmpty(_run.Steps))
        {
            _stepsService.GenerateOriginSteps(_run, _character);
            UpdateAllStepsVisuals();
        }
    }

    private void UpdateAllStepsVisuals()
    {
        if (_stepsContainer != null)
            Destroy(_stepsContainer);
        _stepsContainer = Instantiator.NewStepsContainer();
        var steps = _stepsService.GetAllSteps(_run);
        foreach (Step step in steps)
        {
            var stepInstance = Instantiator.NewStepInstance(step);
            stepInstance.transform.parent = _stepsContainer.transform;
        }
    }
}
